using ASP_PV411.Data;
using ASP_PV411.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Controllers
{
    public class ShopController(DataContext dataContext) : Controller
    {
        public IActionResult Index()
        {
            ShopIndexViewModel model = new()
            {
                Groups = dataContext.Groups.Where(g => g.DeleteAt == null).ToList(),
            };

            return View(model);
        }
        
        public IActionResult Group([FromRoute] string id)
        {
            ShopGroupViewModel model = new()
            {
                Group = dataContext
                .Groups
                .Include(g => g.Products)
                .FirstOrDefault(g => g.Slug == id)
            };

            return View(model);
        }
    }
}
