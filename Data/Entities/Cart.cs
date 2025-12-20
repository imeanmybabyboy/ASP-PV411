namespace ASP_PV411.Data.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? DiscountId { get; set; }
        public double Price { get; set; }
        public DateTime OpenAt { get; set; } = DateTime.Now;
        public DateTime? CloseAt { get; set; }
        public int? CloseStatus { get; set; } // 1 - purchased, -1 canceled, null - current cart

        public ICollection<CartItem> CartItems { get; set; } = [];
        public User User { get; set; } = null!;
        public Discount? Discount { get; set; }
    }
}
