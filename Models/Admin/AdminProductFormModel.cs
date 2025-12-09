using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Models.Admin
{
    public class AdminProductFormModel
    {
        [FromForm(Name = "admin-product-group")]
        public string GroupId { get; set; } = null!;
        
        
        [FromForm(Name = "admin-product-manufacturer")]
        public string ManufacturerId { get; set; } = null!;
      
        
        [FromForm(Name = "admin-product-name")]
        public string Name { get; set; } = null!;


        [FromForm(Name = "admin-product-description")]
        public string Description { get; set; } = null!;


        [FromForm(Name = "admin-product-slug")]
        public string? Slug { get; set; }


        [FromForm(Name = "admin-product-image")]
        public IFormFile Image { get; set; } = null!;


        [FromForm(Name = "admin-product-price")]
        public double Price { get; set; }


        [FromForm(Name = "admin-product-stock")]
        public int Stock { get; set; }
    }
}
