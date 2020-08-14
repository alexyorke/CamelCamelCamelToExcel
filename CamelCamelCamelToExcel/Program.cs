using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace CamelCamelCamelToExcel
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string url = "https://camelcamelcamel.com/product/B07G82D89J?context=search";
            var scaledGraph = Graph.CreateGraphFromUrl(url);

            var output = scaledGraph.Aggregate("", (current, point) => current + $"{point.X}\t{point.Y}\n");

            File.WriteAllText("camelcamelcamel.txt", output);
        }

        public static Graph DownloadGraph(string url, DateTime startDate, DateTime? endDate = null)
        {
            var contents = DownloadWebpage(url);
            var imageUrl = Graph.ConvertWebpageUrlToImageUrl(url);
            var downloadedGraph = DownloadImage(imageUrl);
            return new Graph
            {
                graph = downloadedGraph, startDate = startDate, endDate = endDate ?? DateTime.Now,
                maxPrice = 0, minPrice = 0
            };
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
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                             "Windows NT 5.2; .NET CLR 1.0.3705;)");
            var stream = client.OpenRead(url);
            return new Bitmap(stream);
        }
    }
}