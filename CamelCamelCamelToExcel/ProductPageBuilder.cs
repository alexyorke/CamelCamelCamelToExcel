using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace CamelCamelCamelToExcel
{
    internal class ProductPageBuilder
    {
        private readonly string _url;

        public ProductPageBuilder(string url)
        {
            this._url = url;
        }

        public ProductPage Build()
        {
            var web = new HtmlWeb();
            var doc = web.Load(_url);

            var MaxPrice = Convert.ToDecimal(doc.DocumentNode.SelectNodes("//div[1]/div[@class=' margined' and 1]/div[@class='row column' and 2]/div[@class='row' and 1]/div[1]/table[@class='product_pane' and 1]/tbody[1]/tr[@class='highest_price' and 2]/td[2]").Nodes().FirstOrDefault()?.InnerText?.Replace("$", ""));
            var MinPrice = Convert.ToDecimal(doc.DocumentNode.SelectNodes("//div[1]/div[@class=' margined' and 1]/div[@class='row column' and 2]/div[@class='row' and 1]/div[1]/table[@class='product_pane' and 1]/tbody[1]/tr[@class='lowest_price' and 3]/td[2]").Nodes().FirstOrDefault()?.InnerText?.Replace("$", ""));

            var StartDateRaw = string.Join("", doc.DocumentNode
                .SelectNodes(
                    "//div[1]/div[@class=' margined' and 1]/div[@class='row column' and 2]/div[@class='row' and 1]/div[1]/p[@class='grey' and 1]/small[1]")
                .Nodes().Select(x => x.InnerText).ToList());

            var StartDate = DateTime.Parse(Regex.Match(StartDateRaw, @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sept|Oct|Nov|Dec) \d+\, \d+").Value);

            var graphParameters = new GraphParameters(StartDate, DateTime.Now, 1000, 1000);
            var graph = new Graph(_url, graphParameters, MinPrice, MaxPrice);

            return new ProductPage {Url = _url, Graph = graph, MinPrice = MinPrice, MaxPrice = MaxPrice };
        }
    }
}