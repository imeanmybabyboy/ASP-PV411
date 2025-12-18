namespace ASP_PV411.Models.Cart
{
    public class CartIndexViewModel
    {
        public bool IsAuthorized { get; set; }
        public Data.Entities.Cart? ActiveCart { get; set; }
    }
}
