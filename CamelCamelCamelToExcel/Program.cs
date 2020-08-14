using System;
using System.IO;
using System.Linq;

namespace CamelCamelCamelToExcel
{
    internal class Program
    {
        private static void Main()
        {
            const string url = "https://camelcamelcamel.com/product/B07G82D89J?context=search";
            var product = PriceHistory.FromUrl(url);
            var scaledGraph = new PriceHistory(product);
            var output2 = scaledGraph.Create();

            var output = output2.Aggregate("", (current, point) => current + $"{point.X}\t{point.Y}\n");

            File.WriteAllText("camelcamelcamel.txt", output);
        }
    }
}