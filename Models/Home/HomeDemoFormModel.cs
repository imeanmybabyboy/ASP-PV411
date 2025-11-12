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
        public DateOnly? UserBirthdate { get; set; } = null;
        
        
        [FromForm(Name = "agreement")]
        [Required(ErrorMessage = "Необхідно погодитися з правилами сайту")]
        public string? IsAgree { get; set; } = null;
        
        
        [FromForm(Name = "user-phone-number")]
        [Required(ErrorMessage = "Необхідно зазначити номер телефону")]
        public string UserPhoneNumber { get; set; } = null!;

    }
}
