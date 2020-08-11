using System.Drawing;

namespace CamelCamelCamelToExcel
{
    internal static class Utils
    {
        // https://stats.stackexchange.com/questions/281162/
        public static PointF ScalePoint(PointF i, decimal xMin, decimal xMax, decimal yMin, decimal yMax,
            decimal tyMin, decimal tyMax, decimal txMin, decimal txMax)
        {
            var m = new PointF(i.X, i.Y);

            m.X = (float) ((m.X - (double) xMin) / (double) (xMax - xMin) * (double) (txMax - txMin) + (double) txMin);
            m.Y = (float) ((m.Y - (double) yMin) / (double) (yMax - yMin) * (double) (tyMax - tyMin) + (double) tyMin);

            return m;
        }
    }
}