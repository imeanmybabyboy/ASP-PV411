namespace ASP_PV411.Models.Shop
{
    public class ShopProductViewModel
    {
        public Data.Entities.Product? Product { get; set; }
        public List<Data.Entities.Product>? YouMayLikeProducts { get; set; }
    }
}
