using ASP_PV411.Services.FileExtensionChecker;
using ASP_PV411.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class StorageController(IStorageService storageService, IFileExtensionCheckerService fileExtensionCheckerService) : Controller
    {
        public IActionResult Item([FromRoute] string id)
        {
            int dotIndex = id.LastIndexOf('.');
            if (dotIndex == -1)
            {
                return NotFound("File item must have an extension");
            }
            string? ext = id[dotIndex..];
            string mediaType = fileExtensionCheckerService.GetValidMediaType(ext);

            if (mediaType == null)
            {
                return new UnsupportedMediaTypeResult();
            }
            
            try
            {
                return File(storageService.Get(id), mediaType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
