namespace CamelCamelCamelToExcel
{
    internal class ProductPage
    {
        public Graph Graph
        {
            get;
            internal set;
        }

        public string Url { get; set; }
        public decimal MinPrice { get; internal set; }
        public decimal MaxPrice { get; internal set; }
    }
}