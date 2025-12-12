namespace ASP_PV411.Data.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? DiscountId { get; set; }
        public int Quantity { get; set; } = 1;
        public double Price { get; set; }
        public DateTime OpenAt { get; set; } = DateTime.Now;
        public DateTime? DeleteAt { get; set; }

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Discount? Discount { get; set; }
    }
}
