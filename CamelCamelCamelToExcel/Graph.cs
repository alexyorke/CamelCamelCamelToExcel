using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace CamelCamelCamelToExcel
{
    internal class Graph
    {
        private readonly GraphParameters _graphParameters;
        private readonly string _url;

        public Graph(string url, GraphParameters graphParameters, decimal minprice, decimal maxprice)
        {
            _url = url;
            _graphParameters = graphParameters;
            this.MinPrice = minprice;
            this.MaxPrice = maxprice;
        }

        private decimal MinPrice { get; set; }

        private decimal MaxPrice { get; set; }

        public IEnumerable<PointF> Create()
        {
            var image = DownloadGraphImage(_url, _graphParameters);
            var graphPoints = GetPricePoints(image);

            var graphAtOriginPoints = AlignGraphToOrigin(graphPoints, image);

            var startDate = this._graphParameters.StartDate;
            var endDate = this._graphParameters.EndDate;
            var maxDay = (endDate - startDate).Days;

            // scale graph to min max price and days
            return ResizeGraphToFitArea(graphAtOriginPoints, this.MinPrice, this.MaxPrice, maxDay);
        }

        private static IEnumerable<PointF> ResizeGraphToFitArea(IReadOnlyCollection<PointF> graphAtOriginPoints,
            decimal minPrice, decimal maxPrice,
            int maxDay)
        {
            var scaledGraph = graphAtOriginPoints.Select(p => ScalePoint(p,
                (decimal) graphAtOriginPoints.Min(q => q.X),
                (decimal) graphAtOriginPoints.Max(q => q.X),
                (decimal) graphAtOriginPoints.Min(q => q.Y),
                (decimal) graphAtOriginPoints.Max(q => q.Y),
                minPrice, maxPrice, 0, maxDay));
            return scaledGraph;
        }

        private static List<PointF> AlignGraphToOrigin(IEnumerable<PointF> graphPoints, Image image)
        {
            var graphAtOriginPoints = graphPoints.Select(p => new PointF(p.X, image.Height - p.Y - 1))
                // retain topmost y values
                .GroupBy(q => q.X, q => q.Y, (key, g)
                    => new PointF {X = key, Y = g.Max()})
                .ToList();
            return graphAtOriginPoints;
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

        private static string ConvertWebpageUrlToImageUrl(string url, uint width, uint height)
        {
            var productId = Regex.Match(url, @"product\/([A-Za-z0-9]+)").Groups[1].Value;
            return
                $"https://charts.camelcamelcamel.com/us/{productId}/amazon.png?force=1&zero=0&w={width}&h={height}&desired=false&legend=1&ilt=1&tp=all&fo=0&lang=en";
        }

        private static Bitmap DownloadGraphImage(string url, GraphParameters parameters)
        {
            var imageUrl = ConvertWebpageUrlToImageUrl(url, parameters.Width, parameters.Height);
            return HttpService.DownloadImage(imageUrl);
        }

        private static PointF ScalePoint(PointF i, decimal xMin, decimal xMax, decimal yMin, decimal yMax,
            decimal tyMin, decimal tyMax, decimal txMin, decimal txMax)
        {
            var m = new PointF(i.X, i.Y);

            m.X = (float) ((m.X - (double) xMin) / (double) (xMax - xMin) * (double) (txMax - txMin) + (double) txMin);
            m.Y = (float) ((m.Y - (double) yMin) / (double) (yMax - yMin) * (double) (tyMax - tyMin) + (double) tyMin);

            return m;
        }
    }
}