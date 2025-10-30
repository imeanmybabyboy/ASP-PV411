namespace ASP_PV411.Models.Home
{
    public class HomeProductsViewModel
    {
        public List<Product> Products { get; set; } = [];
    }

    public class Product
    {
        public string? Name { get; set; } = null;
        public double? Price { get; set; } = 0;
    }
}
