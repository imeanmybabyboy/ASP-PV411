using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ASP_PV411.Models.Admin
{
    public class AdminManufacturerFormModel
    {
        [FromForm(Name = "admin-manufacturer-name")]
        [Required(ErrorMessage = "Необхідно ввести назву виробника")]
        public string Name { get; set; } = null!;


        [FromForm(Name = "admin-manufacturer-description")]
        public string Description { get; set; } = null!;


        [FromForm(Name = "admin-manufacturer-image")]
        public IFormFile Image { get; set; } = null!;


        [FromForm(Name = "admin-manufacturer-slug")]
        public string Slug { get; set; } = null!;
    }
}
