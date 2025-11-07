using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_PV411.Models.Home
{
    public class HomeDemoFormModel
    {
        [FromForm(Name = "user-name")]
        [Required(ErrorMessage = "Необхідно зазначити ім'я")]
        public string UserName { get; set; } = null!;
        
        [FromForm(Name = "user-email")]
        public string UserEmail { get; set; } = null!;

    }
}
