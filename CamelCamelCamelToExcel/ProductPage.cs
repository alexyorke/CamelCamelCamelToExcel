namespace CamelCamelCamelToExcel
{
    internal class ProductPage
    {
        /// <summary>
        ///     The CamelCamelCamel price history graph
        /// </summary>
        public Graph Graph { get; internal set; }

        public string Url { get; set; }
        public decimal MinPrice { get; internal set; }
        public decimal MaxPrice { get; internal set; }
    }
}