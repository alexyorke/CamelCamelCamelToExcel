using System;

namespace CamelCamelCamelToExcel
{
    internal class GraphParameters
    {
        public DateTime EndDate;
        public DateTime StartDate;
        public readonly uint Width;
        public readonly uint Height;

        public GraphParameters()
        {
            StartDate = DateTime.Now - TimeSpan.FromDays(365);
            EndDate = DateTime.Now;
            Width = 1000;
            Height = 1000;
        }

        public GraphParameters(DateTime startDate, DateTime endDate, uint width, uint height)
        {
            StartDate = startDate;
            EndDate = endDate;
            Width = width;
            Height = height;
        }

        public GraphParameters(DateTime startDate)
        {
            StartDate = startDate;
            EndDate = DateTime.Now;
            Width = 1000;
            Height = 1000;
        }
    }
}