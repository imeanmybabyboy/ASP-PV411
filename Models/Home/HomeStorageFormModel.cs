using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Models.Home
{
    public class HomeStorageFormModel
    {
        [FromForm(Name = "form-file")]
        public IFormFile FormFile { get; set; } = null!;
    }
}
