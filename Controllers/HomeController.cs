using System.Diagnostics;
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
            ViewBag.arr1 = new String[] { "string1", "string2", "string3" };

            // dictionary
            ViewData["arr2"] = new String[] { "string4", "string5", "string6" };

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
