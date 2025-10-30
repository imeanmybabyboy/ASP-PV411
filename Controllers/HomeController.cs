using System.Diagnostics;
using ASP_PV411.Models;
using ASP_PV411.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
