using ASP_PV411.Data;
using ASP_PV411.Data.Entities;
using ASP_PV411.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_PV411.Controllers
{
    public class CartController(DataContext dataContext) : Controller
    {
        public IActionResult Index([FromRoute] string? id)
        {
            string? userId = User
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            
            CartIndexViewModel viewModel = new()
            {
                IsAuthorized = userId != null,
                ActiveCart = userId == null ? null :
                dataContext
                .Carts
                .Include(c => c.Discount)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId.ToString() == userId && (id == null ? c.CloseAt == null : c.Id.ToString() == id))
            };

            return View(viewModel);
        }

        public JsonResult Buy([FromRoute] string id)
        {
            try
            {
                Cart cart = GetCartById(id);

                cart.CloseAt = DateTime.Now;
                cart.CloseStatus = 1;
                dataContext.SaveChanges();
                
                return Json(new { Status = "Ok", Message = "Complete" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = ex.Message });
            }
        }

        public JsonResult Drop([FromRoute] string id)
        {
            try
            {
                Cart cart = GetCartById(id);

                cart.CloseAt = DateTime.Now;
                cart.CloseStatus = -1;
                dataContext.SaveChanges();

                return Json(new { Status = "Ok", Message = "Complete" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = ex.Message });
            }
        }

        public JsonResult Add([FromRoute] String id)
        {
            Guid? userId = GetAuthUserId();

            if (userId == null)
            {
                Json(new { Status = "Error", Message = "You need to authenticate" });
            }

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
                    UserId = userId!.Value,
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

        private Guid? GetAuthUserId()
        {
            // Перевіряємо авторизацію, вилучаємо ід користувача
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return null;
            }
            try
            {
                return Guid.Parse(
                    HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value
                );
            }
            catch (Exception)
            {
                return null;
            }

        }

        private Cart GetCartById(string id)
        {
            Guid? userId = GetAuthUserId()
                ?? throw new Exception("You need to authenticate");

            // Перевіряємо валідність кошику
            Cart? cart = dataContext.Carts.FirstOrDefault(c => c.Id.ToString() == id)
                ?? throw new Exception("Cart not found");

            // Перевіряємо, що кошик активний (не закритий), а також його належність користувачеві
            if (cart.UserId != userId)
            {
                throw new Exception("Forbidden");
            }

            if (cart.CloseAt != null)
            {
                throw new Exception("Cart is closed");
            }

            return cart;
        }

    }
}
