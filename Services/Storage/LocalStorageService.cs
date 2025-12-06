
using ASP_PV411.Services.FileExtensionChecker;
using Microsoft.AspNetCore.Http;

namespace ASP_PV411.Services.Storage
{
    public class LocalStorageService(IFileExtensionCheckerService fileExtensionCheckerService) : IStorageService
    {
        private readonly string _storagePath = @"D:\IT\Project ITstep\Full Stack\ASP_Net\storage";

        public byte[] Get(string fileName)
        {
            string path = Path.Combine(_storagePath, fileName);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public string Save(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                throw new ArgumentNullException(nameof(formFile));
            }

            // файли не можна зберігати з тим іменем, що приходить по запиту, оскільки можливі конфлікти (вже є такий файл), а також з міркувань безпеки
            // Генеруємо нове ім'я, зберігаючи розширення файлу

            string mediaType = fileExtensionCheckerService.GetValidMediaType(formFile.FileName);

            if (mediaType == null)
            {
                throw new Exception("Розширення файлу не відповідає розширенню картинки!");
            }

            string ext = "." + mediaType.Split('/')[1];

            string savedName = Guid.NewGuid().ToString() + ext;
            using var wStream = new StreamWriter(Path.Combine(_storagePath, savedName));

            formFile.CopyTo(wStream.BaseStream);

            return savedName;
        }
    }
}
