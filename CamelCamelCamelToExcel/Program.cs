using System;
using System.IO;
using System.Linq;

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
    }
}