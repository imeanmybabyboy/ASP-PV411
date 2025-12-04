using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_PV411.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            string? roleId = isAuthenticated
                ? HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                : null;
            if (roleId != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
