using ASP_PV411.Data;
using ASP_PV411.Models.Rest;
using ASP_PV411.Models.Shop;
using ASP_PV411.Services.Random;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Controllers
{
    public class ShopController(DataContext dataContext, IRandomService randomService, DataAccessor dataAccessor) : Controller
    {
        private string FullImageUrl(string localUrl) => $"{Request.Scheme}://{Request.Host}/Storage/Item/{localUrl}";

        public IActionResult Index()
        {
            ShopIndexViewModel model = new()
            {
                Groups = dataContext.Groups.Where(g => g.DeleteAt == null).ToList(),
            };

            return View(model);
        }

        public JsonResult ApiIndex()
        {
            ShopIndexViewModel model = new()
            {
                Groups = dataContext
                .Groups
                .Where(g => g.DeleteAt == null)
                .AsEnumerable()
                .Select(g => g with
                {
                    ImageUrl = FullImageUrl(g.ImageUrl)
                })
                .ToList()
            };

            return Json(model);
        }

        public IActionResult Group([FromRoute] string id)
        {
            // якщо товарів немає, обираємо рандомні товари з іншої категорії, кількість товарів якої > 0
            int currentGroupProductsQuantity = dataContext.Groups.Include(g => g.Products).Where(g => g.Slug == id).Select(g => g.Products.Count).FirstOrDefault();
            ICollection<Data.Entities.Product> youMayLikeProducts = null!;

            // перевіряємо, щоб група була порожня для формування списку товарів, які можуть сподобатися
            if (currentGroupProductsQuantity == 0)
            {
                // беремо Id поточної групи та звіряємо, щоб рандомна група != поточній
                string currentGroupId = dataContext
                        .Groups
                        .FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id)!
                        .Id.ToString()!;

                var groups = dataContext
                    .Groups
                    .Where(g => g.Id != Guid.Parse(currentGroupId) && g.Products.Count != 0)
                    .ToList();

                string randomGroupId = groups[randomService.RandomInt(groups.Count)].Id.ToString();

                // створюємо список товарів із рандомної групи
                youMayLikeProducts = dataContext.Products.Where(p => p.GroupId == Guid.Parse(randomGroupId)).ToList();
            }

            ShopGroupViewModel model = new()
            {
                Group = dataAccessor.GetGroupBySlug(id),
                YouMayLikeProducts = youMayLikeProducts,
            };

            return View(model);
        }

        public JsonResult ApiGroup([FromRoute] string id)
        {
            var group = dataAccessor.GetGroupBySlug(id);

            if (group != null)
            {
                group = group with
                {
                    ImageUrl = FullImageUrl(group.ImageUrl),
                    Products = [..group.Products.Select(p => p with
                    {
                        ImageUrl = p.ImageUrl == null ? null : FullImageUrl(p.ImageUrl)
                    })],
                };
            }

            RestResponse restResponse = new()
            {
                Status = group == null ? RestStatus.NotFound : RestStatus.Ok,
                Meta = new()
                {
                    Service = "Крамниця",
                    ServerTime = DateTime.Now.Ticks,
                    Cache = 86400,
                    DataType = group == null ? "null" : "object",
                    Method = Request.Method,
                    Path = Request.Path,
                    Resouce = "Product group",
                },
                Data = group,
            };
            return Json(restResponse);
        }

        public IActionResult Product([FromRoute] string id)
        {
            string currentGroupId = dataContext.Products.FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id)!.GroupId.ToString();
            var currentProduct = dataContext
                .Products
                .FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id);

            // обираємо інші товари з цієї ж категорії
            var youMayLikeProducts = dataContext.Products.Where(p => p.GroupId == Guid.Parse(currentGroupId) && p.Id != currentProduct!.Id).ToList();

            // якщо товарів у цій категорії більше немає, обираємо товари з іншої категорії
            if (youMayLikeProducts.Count == 0)
            {
                var groups = dataContext
                    .Groups
                    .Where(g => g.Id != Guid.Parse(currentGroupId) && g.Products.Count != 0)
                    .ToList();

                string randomGroupId = groups[randomService.RandomInt(groups.Count)].Id.ToString();
                youMayLikeProducts = dataContext.Products.Where(p => p.GroupId == Guid.Parse(randomGroupId)).ToList();
            }

            ShopProductViewModel model = new()
            {
                Product = currentProduct,
                YouMayLikeProducts = youMayLikeProducts,
            };

            return View(model);
        }
    }
}


/*
REST
{
    "status": {
        "isOk": false,
        "code": 401,
        "phrase": "Unauthorized"
    },
    "meta": {
        
}

}

 
 */