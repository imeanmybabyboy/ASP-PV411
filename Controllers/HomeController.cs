using ASP_PV411.Models;
using ASP_PV411.Models.Home;
using ASP_PV411.Services.FolderName;
using ASP_PV411.Services.Hash;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.OTP;
using ASP_PV411.Services.Random;
using ASP_PV411.Services.Salt;
using ASP_PV411.Services.Signature;
using ASP_PV411.Services.Timestamp;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASP_PV411.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRandomService _randomService;
        private readonly ITimestampService _timestampService;
        private readonly IHashService _hashService;
        private readonly ISaltService _saltService;
        private readonly IKdfService _kdfService;
        private readonly ISignatureService _signatureService;
        private readonly IOtpService _otpService;
        private readonly IFolderNameService _folderNameService;

        public HomeController(ILogger<HomeController> logger, IRandomService randomService, ITimestampService timestampService, IHashService hashService, ISaltService saltService, IKdfService kdfService, ISignatureService signatureService, IOtpService otpService, IFolderNameService folderNameService)
        {
            _logger = logger;
            _randomService = randomService;
            _timestampService = timestampService;
            _hashService = hashService;
            _saltService = saltService;
            _kdfService = kdfService;
            _signatureService = signatureService;
            _otpService = otpService;
            _folderNameService = folderNameService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FormsHomework([FromQuery] List<string>? name)
        {
            if (name != null)
            {
                ViewData["fullName"] = string.Join(' ', name);
                if (HttpContext.Session.Keys.Contains("fullName"))
                {
                    ViewData["post-fullName"] = HttpContext.Session.GetString("fullName");
                    HttpContext.Session.Remove("fullName");
                }
            }

            return View();
        }

        public IActionResult FormPostFullName([FromForm] List<string> name)
        {
            string fullName = string.Join(' ', name);

            HttpContext.Session.SetString("fullName", fullName);

            return RedirectToAction(nameof(FormsHomework));
        }

        public IActionResult Forms([FromQuery] string? data)
        {
            /* Binding (model binding) - процес в ASP, який автоматично зв'язує параметри, що передаються у запиті, із аргументами із параметрами методів контролера. Якщо зв'язування не вдається (виникають помилки), то автоматично формується відповідь-помилка.
             * Зв'язування відбувається за іменами - назва параметра методу має збігатися із назвою параметра, що приходить з форми*/
            ViewData["data"] = data;

            /* Якщо було надсилання post-даних, то при переході (Redirect) на даний метод у сесії буде наявний параметр "data". Слід перевірити його існування, за наявності обробити та видалити з сесії, щоб при повторному заході він вже не оброблявся.
             */

            if (HttpContext.Session.Keys.Contains("data"))
            {
                ViewData["post-data"] = HttpContext.Session.GetString("data");
                HttpContext.Session.Remove("data");
            }

            return View();
        }

        public IActionResult FormPost([FromForm] string data)
        {
            /* Проблема з Post-даними виникає, коли користувач намагається оновити сторінку браузера, що побудована за результатами надсилання форми - брузер видає попередженння, що не стилізується та може спантеличити користувача.
             * Застосовується наступна схема:
             * BROWSER                                                   SERVER                     Session
             * - запускається метод post data = 123 ---------> зберігає дані (data = 123) --------> data = 123
             *                                                      Redirect                         |   
             *         <---------------------------------------                                      |
             *                                  і повідомляє про необхідність переходу на іншу       |
             *                                  адресу                                               |
             * GET                                                                                   |
             *     ----------------------------> Відновлюємо збережені дані                          |
             *                                   та формуємо результат                               data = 123
             */

            HttpContext.Session.SetString("data", data);

            return RedirectToAction(nameof(Forms));
        }

        public IActionResult Services()
        {
            ViewData["password"] = $"Default length (6): {_otpService.GetOneTimePassword()} | " +
                $"Custom length (4): {_otpService.GetOneTimePassword(4)}";

            ViewData["randomFolderName"] = _folderNameService.Name();

            return View();
        }

        public IActionResult IoC()
        {

            ViewData["rnd"] = _randomService.RandomInt(2000);
            ViewData["ref"] = _randomService.GetHashCode();
            ViewData["ctrl"] = this.GetHashCode();
            ViewData["hash"] = _hashService.Digest("123");
            ViewData["salt"] = _saltService.GetSalt() + " " + _saltService.GetSalt(8);
            ViewData["dk"] = _kdfService.Dk("123", "456");
            ViewData["signature"] = _signatureService.Sign("123", "456");

            return View();
        }

        public IActionResult IoCHomework()
        {
            HomeIocViewModel iocViewModel = new()
            {
                TimestampServiceControllerHashCode = _timestampService.GetHashCode(),
                ServiceType = "Singleton"
            };

            return View(iocViewModel);
        }

        public IActionResult Intro()
        {
            return View();
        }

        public IActionResult Razor()
        {
            // обмінні об'єкти, що дозволяють передати дані від контролера до представлення
            // dynamic
            ViewBag.arr1 = new string[] { "string1", "string2", "string3" };

            // dictionary
            ViewData["arr2"] = new string[] { "string4", "string5", "string6" };

            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Products()
        {
            HomeProductsViewModel model = new()
            {
                Products = [
                    new() {Name = "Asus", Price = 18900},
                    new() {Name = "Lenovo", Price = 19800},
                    new() {Name = "Acer", Price = 21000},
                    new() {Name = "Dell", Price = 25000},
                    new() {Name = "HP", Price = 15200},
                    ]
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
