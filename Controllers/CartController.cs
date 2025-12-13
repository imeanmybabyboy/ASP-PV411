using ASP_PV411.Data;
using ASP_PV411.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_PV411.Controllers
{
    public class CartController(DataContext dataContext) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Add([FromRoute] String id)
        {
            // Перевіряємо авторизацію, вилучаємо ід користувача
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return Json(new { Status = "Error", Message = "You need to authenticate" });
            }
            Guid userId = Guid.Parse(
                HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value
            );

            // Перевіряємо валідність товару (ід)
            var product = dataContext.Products.FirstOrDefault(p => p.Id.ToString() == id);
            if (product == null)
            {
                return Json(new { Status = "Error", Message = "Product not found" });
            }

            // Перевіряємо чи є в користувача відкритий кошик
            // якщо ні, то створюємо новий, якщо є - працюємо з ним

            Cart? cart = dataContext.Carts
                .Include(c => c.Discount)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Discount)

                .FirstOrDefault(c => c.UserId == userId && c.CloseAt == null);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                };
                dataContext.Carts.Add(cart);
            }


            // Перевіряємо чи є в кошику даний товар, якщо є, то
            // збільшуємо кількість, якщо ні - створюємо нову позицію

            CartItem? cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == product.Id && ci.DeleteAt == null);

            if (cartItem == null)
            {
                cartItem = new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    CartId = cart.Id,
                    Quantity = 1,
                    Product = product
                };

                dataContext.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
            }


            // Перераховуємо ціни з урахуванням акцій

            cart.Price = 0.0;
            foreach (CartItem ci in cart.CartItems)
            {
                if (ci.Discount == null)
                {
                    ci.Price = ci.Product.Price * ci.Quantity;
                }
                else
                {
                    ci.Price = ci.Product.Price * ci.Quantity * (ci.Discount.Percent ?? 1.0);
                }
                cart.Price += ci.Price;
            }

            if (cart.DiscountId != null)
            {
                // корегуємо на акцію кошику
            }

            dataContext.SaveChanges();

            // Повертаємо статус обробки

            return Json(new
            {
                Status = "Ok",
                Message = "Successfully added"
            });
        }

    }
}
