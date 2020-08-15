using System.IO;
using System.Linq;

namespace CamelCamelCamelToExcel
{
    internal class Program
    {
        private static void Main()
        {
            const string url = "https://camelcamelcamel.com/product/B07G82D89J?context=search";
            var productPageBuilder = new ProductPageBuilder(url);
            var productPage = productPageBuilder.Build();

            var graph = productPage.Graph.Create();

            var tsv = graph.Aggregate("", (current, point) => current + $"{point.X}\t{point.Y}\n");

            File.WriteAllText("CamelCamelCamelGraph.tsv", tsv);
        }
    }
}