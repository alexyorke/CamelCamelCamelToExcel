using System;

namespace CamelCamelCamelToExcel
{
    internal class GraphParameters
    {
        public DateTime EndDate;
        public DateTime StartDate;
        public readonly uint Width;
        public readonly uint Height;

        public GraphParameters(DateTime startDate, DateTime endDate, uint width, uint height)
        {
            StartDate = startDate;
            EndDate = endDate;
            Width = width;
            Height = height;
        }
    }
}