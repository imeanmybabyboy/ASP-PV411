using ASP_PV411.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Mvc()
        {
            return Content("MvcInfoController::MvcInfo");
        }

        public IActionResult Api()
        {
            return Content("MvcInfoController::ApiInfo");
        }
    }
}
