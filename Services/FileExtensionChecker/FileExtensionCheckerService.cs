
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Services.FileExtensionChecker
{
    public class FileExtensionCheckerService : IFileExtensionCheckerService
    {
        public string GetValidMediaType(string fileName)
        {
            int dotIndex = fileName.LastIndexOf('.');
            if (dotIndex == -1)
            {
                return null!;
            }

            string? mediaType = fileName[dotIndex..] switch
            {
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => null
            };

            return mediaType!;
        }
    }
}
