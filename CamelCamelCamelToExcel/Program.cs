using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace CamelCamelCamelToExcel
{
    internal class Graph
    {
        internal DateTime endDate;
        internal Bitmap graph;
        internal decimal maxPrice;
        internal decimal minPrice;
        internal DateTime startDate;
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            const string url = "https://camelcamelcamel.com/product/B07G82D89J?context=search";
            var image = DownloadGraph(url, new DateTime(2020, 8, 10));
            var graphPoints = GetPricePoints(image.graph);

            var graphAtOriginPoints = graphPoints
                // flip vertically
                .Select(p => new PointF(p.X, image.graph.Height - p.Y - 1))
                // retain topmost y values
                .GroupBy(q => q.X, q => q.Y, (key, g)
                    => new PointF {X = key, Y = g.Max()})
                .ToList();

            const decimal minPrice = 52;
            const decimal maxPrice = 70;
            var startDate = new DateTime(2019, 7, 15);
            var endDate = new DateTime(2020, 7, 15);
            var maxDay = (endDate - startDate).Days;

            // scale graph to min max price and days
            var scaledGraph = graphAtOriginPoints.Select(p => Utils.ScalePoint(p, (decimal) graphAtOriginPoints.Min(q => q.X),
                (decimal) graphAtOriginPoints.Max(q => q.X),
                (decimal) graphAtOriginPoints.Min(q => q.Y),
                (decimal) graphAtOriginPoints.Max(q => q.Y),
                minPrice, maxPrice, 0, maxDay));

            var output = scaledGraph.Aggregate("", (current, point) => current + $"{point.X}\t{point.Y}\n");

            File.WriteAllText("camelcamelcamel.txt", output);
        }

        private static Graph DownloadGraph(string url, DateTime startDate, DateTime? endDate = null)
        {
            var contents = DownloadWebpage(url);
            var imageUrl = ConvertWebpageUrlToImageUrl(contents);
            var downloadedGraph = DownloadImage(imageUrl);
            return new Graph
            {
                graph = downloadedGraph, startDate = startDate, endDate = endDate ?? DateTime.Now,
                maxPrice = 0, minPrice = 0
            };
        }

        private static string ConvertWebpageUrlToImageUrl(string contents)
        {
            var productId = Regex.Match(contents, @"product\/([A-Za-z0-9]+)").Captures[1].Value;
            return "https://charts.camelcamelcamel.com/us/" + productId + "/amazon.png?force=1&zero=0&w=10000&h=10000&desired=false&legend=1&ilt=1&tp=all&fo=0&lang=en";
        }

        private static IEnumerable<PointF> GetPricePoints(Bitmap image)
        {
            var graphPoints = new List<Point>();
            var topLeftHandCorner = new Point();
            var bottomRightHandCorner = new Point();
            for (var x = 0; x < image.Width - 1; x++)
            for (var y = 0; y < image.Height - 1; y++)
            {
                // crop image to just show graph
                if (!topLeftHandCorner.IsEmpty && x < topLeftHandCorner.X && y < topLeftHandCorner.Y) continue;
                if (!bottomRightHandCorner.IsEmpty && x > bottomRightHandCorner.X &&
                    y > bottomRightHandCorner.Y) continue;
                var currPixel = image.GetPixel(x, y);
                if (topLeftHandCorner.IsEmpty && currPixel.Equals(Config.LeftLineColor))
                    topLeftHandCorner = new Point(x, y);
                else if (currPixel.Equals(Config.LeftLineColor) && x >= bottomRightHandCorner.X &&
                         y >= bottomRightHandCorner.Y)
                    bottomRightHandCorner = new Point(x, y);
                else if (currPixel.Equals(Config.GraphPointColor)) graphPoints.Add(new Point(x, y));
            }

            // crop graph to just show graph
            return graphPoints.Select(point =>
                new PointF(point.X - topLeftHandCorner.X, point.Y - topLeftHandCorner.Y));
        }

        private static string DownloadWebpage(string url)
        {
            using var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                             "Windows NT 5.2; .NET CLR 1.0.3705;)");
            var stream = client.OpenRead(url);
            using var reader =
                new StreamReader(stream ?? throw new InvalidOperationException("Webpage could not be downloaded"));
            return reader.ReadToEnd();
        }

        private static Bitmap DownloadImage(string url)
        {
            using var client = new WebClient();
            var stream = client.OpenRead(url);
            return new Bitmap(stream);
        }
    }
}