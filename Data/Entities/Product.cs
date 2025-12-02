using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_PV411.Data.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid ManufacturerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Descirption { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;
        public int Stock { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public double Price { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}
