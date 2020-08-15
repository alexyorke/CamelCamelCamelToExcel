using System;

namespace CamelCamelCamelToExcel
{
    /// <summary>
    ///     Graph configuration options
    /// </summary>
    internal class GraphParameters
    {
        public readonly uint Height;
        public readonly uint Width;
        public DateTime EndDate;
        public DateTime StartDate;

        public GraphParameters(DateTime startDate, DateTime endDate, uint width, uint height)
        {
            StartDate = startDate;
            EndDate = endDate;
            Width = width;
            Height = height;
        }
    }
}