using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace CamelCamelCamelToExcel
{
    internal class Graph
    {
        private DateTime endDate;
        private Bitmap graph;
        private decimal maxPrice;
        private decimal minPrice;
        private DateTime startDate;
        private uint width;
        private uint height;

        public IEnumerable<PointF> Create(string url)
        {
            var image = DownloadGraph(url, new DateTime(2020, 8, 10), DateTime.Now, width, height);
            var graphPoints = GetPricePoints(image.graph);

            var graphAtOriginPoints = graphPoints.Select(p => new PointF(p.X, image.graph.Height - p.Y - 1))
                // retain topmost y values
                .GroupBy(q => q.X, q => q.Y, (key, g)
                    => new PointF {X = key, Y = g.Max()})
                .ToList();

            decimal minPrice = GetMinPrice();
            decimal maxPrice = GetMaxPrice();
            var startDate = GetStartDate();
            var endDate = GetEndDate();
            var maxDay = (endDate - startDate).Days;

            // scale graph to min max price and days
            var scaledGraph = graphAtOriginPoints.Select(p => Utils.ScalePoint(p,
                (decimal) graphAtOriginPoints.Min(q => q.X),
                (decimal) graphAtOriginPoints.Max(q => q.X),
                (decimal) graphAtOriginPoints.Min(q => q.Y),
                (decimal) graphAtOriginPoints.Max(q => q.Y),
                minPrice, maxPrice, 0, maxDay));
            return scaledGraph;
        }

        private static DateTime GetEndDate()
        {
            return new DateTime(2020, 7, 15);
        }

        private static DateTime GetStartDate()
        {
            return new DateTime(2019, 7, 15);
        }

        private static int GetMaxPrice()
        {
            return 70;
        }

        private static int GetMinPrice()
        {
            return 52;
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

        private static string ConvertWebpageUrlToImageUrl(string contents, uint width, uint height)
        {
            var productId = Regex.Match(contents, @"product\/([A-Za-z0-9]+)").Groups[1].Value;
            return
                $"https://charts.camelcamelcamel.com/us/{productId}/amazon.png?force=1&zero=0&w={width}&h={height}&desired=false&legend=1&ilt=1&tp=all&fo=0&lang=en";
        }

        private static Graph DownloadGraph(string url, DateTime startDate, DateTime endDate, uint width, uint height)
        {
            var contents = HttpService.DownloadWebpage(url);
            var imageUrl = Graph.ConvertWebpageUrlToImageUrl(url, width, height);
            var downloadedGraph = HttpService.DownloadImage(imageUrl);
            return new Graph
            {
                graph = downloadedGraph, startDate = startDate, endDate = endDate,
                maxPrice = 0, minPrice = 0
            };
        }
    }
}