using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Models.Admin
{
    public class AdminManufacturerFormModel
    {
        [FromForm(Name = "admin-manufacturer-name")]
        public string Name { get; set; } = null!;


        [FromForm(Name = "admin-manufacturer-description")]
        public string Description { get; set; } = null!;


        [FromForm(Name = "admin-manufacturer-image")]
        public IFormFile Image { get; set; } = null!;
    }
}
