namespace ASP_PV411.Data.Entities
{
    public class Manufacturer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Descirption { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime? DeleteAt { get; set; }
    }
}
