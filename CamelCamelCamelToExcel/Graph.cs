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
            MinPrice = minprice;
            MaxPrice = maxprice;
        }

        private decimal MinPrice { get; }

        private decimal MaxPrice { get; }

        /// <summary>
        ///     Creates a graph
        /// </summary>
        /// <returns>The price graph</returns>
        public IEnumerable<PointF> Create()
        {
            var image = DownloadGraphImage(_url, _graphParameters);
            var graphPoints = GetPricePoints(image);

            var graphAtOriginPoints = AlignGraphToOrigin(graphPoints, image);

            var startDate = _graphParameters.StartDate;
            var endDate = _graphParameters.EndDate;
            var maxDay = (endDate - startDate).Days;

            // scale graph to min max price and days
            return ResizeGraphToFitArea(graphAtOriginPoints, MinPrice, MaxPrice, maxDay);
        }

        /// <summary>
        ///     Resizes a graph in the x and y direction to set a new min and max
        /// </summary>
        /// <param name="graphAtOriginPoints">The aligned graph to (0, 0)</param>
        /// <param name="minPrice">The minimum price</param>
        /// <param name="maxPrice">The maximum price</param>
        /// <param name="maxDay">The total days in the graph</param>
        /// <returns>A new graph which is resized to fit that area</returns>
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

        /// <summary>
        ///     Shifts the graph so that (0, 0) is the top-most point in the graph
        /// </summary>
        /// <param name="graphPoints">A list of graph points</param>
        /// <param name="image">The graph image to use as a reference</param>
        /// <returns>A new graph which is aligned to the origin</returns>
        private static List<PointF> AlignGraphToOrigin(IEnumerable<PointF> graphPoints, Image image)
        {
            var graphAtOriginPoints = graphPoints.Select(p => new PointF(p.X, image.Height - p.Y - 1))
                // retain topmost y values
                .GroupBy(q => q.X, q => q.Y, (key, g)
                    => new PointF {X = key, Y = g.Max()})
                .ToList();
            return graphAtOriginPoints;
        }

        /// <summary>
        ///     Parses a graph image into a list of points
        /// </summary>
        /// <param name="image">The graph image</param>
        /// <returns>A set of points corresponding to raw pixel positions on the image</returns>
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

        /// <summary>
        ///     Converts a webpage url to an image url.
        /// </summary>
        /// <param name="url">The CamelCamelCamel product url</param>
        /// <param name="width">The desired width of the image in pixels</param>
        /// <param name="height">The desired height of the image in pixels</param>
        /// <returns>An image url</returns>
        private static string ConvertWebpageUrlToImageUrl(string url, uint width, uint height)
        {
            var productId = Regex.Match(url, @"product\/([A-Za-z0-9]+)").Groups[1].Value;
            return
                $"https://charts.camelcamelcamel.com/us/{productId}/amazon.png?force=1&zero=0&w={width}&h={height}&desired=false&legend=1&ilt=1&tp=all&fo=0&lang=en";
        }

        /// <summary>
        ///     Downloads a graph using specified graph parameters
        /// </summary>
        /// <param name="url">The product url to get the image from</param>
        /// <param name="parameters">A set of optional graph parameters</param>
        /// <returns>A new image</returns>
        private static Bitmap DownloadGraphImage(string url, GraphParameters parameters)
        {
            var imageUrl = ConvertWebpageUrlToImageUrl(url, parameters.Width, parameters.Height);
            return HttpService.DownloadImage(imageUrl);
        }

        /// <summary>
        ///     Transforms a point on a 2D matrix such that it will exist within that matrix
        /// </summary>
        /// <param name="i">The point to transform</param>
        /// <param name="xMin">The minimum x value</param>
        /// <param name="xMax">The maximum x value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="tyMin">The minimum y value in the "horizontal" direction</param>
        /// <param name="tyMax">The maximum y value in the "horizontal" direction</param>
        /// <param name="txMin">The minimum x value in the "horizontal" direction</param>
        /// <param name="txMax">The maximum x value in the "horizontal" direction</param>
        /// <returns></returns>
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