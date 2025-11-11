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
        [Required(ErrorMessage = "Поле Email не може бути пустим")]
        public string UserEmail { get; set; } = null!;


        [FromForm(Name = "user-password")]
        [Required(ErrorMessage = "Необхідно зазначити пароль")]
        public string UserPassword { get; set; } = null!;


        [FromForm(Name = "user-birthdate")]
        [Required(ErrorMessage = "Необхідно зазначити дату народження")]
        public DateOnly UserBirthdate { get; set; } 

    }
}
