using System;
using System.Collections.Generic;
using System.Drawing;

namespace CamelCamelCamelToExcel
{
    internal class PriceHistory
    {
        private Product product;
        private Graph graph;
        private GraphParameters _parameters;

        public PriceHistory(Product product)
        {
            this.product = product;
        }

        public PriceHistory(Product product, GraphParameters parameters)
        {
            this.product = product;
            _parameters = parameters;
        }

        private Decimal MinPrice { get; set; }
        private Decimal MaxPrice { get; set; }
        private DateTime StartDate { get; set; }
        private DateTime EndDate { get; set; }

        public static Product FromUrl(string url)
        {
            return new Product {url = url};
        }

        public IEnumerable<PointF> Create()
        {
            var webpageHtml = HttpService.DownloadWebpage(this.product.url);
            // parse webPageHtml and set minprice, maxprice, startdate, enddate
            ParseListing(webpageHtml);
            // width and height depend on how close the start and end date are
            _parameters ??= new GraphParameters(StartDate, EndDate, 1000, 1000);
            return new Graph(this.product.url, _parameters, MinPrice, MaxPrice).Create();
        }

        private void ParseListing(string webpageHtml)
        {
            MinPrice = 23;
            MaxPrice = 57;
            StartDate = DateTime.Now - TimeSpan.FromDays(365);
            EndDate = DateTime.Now;
        }
    }
}