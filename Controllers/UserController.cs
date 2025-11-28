using ASP_PV411.Data;
using ASP_PV411.Middleware;
using ASP_PV411.Models.User;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.Salt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ASP_PV411.Controllers
{
    public class UserController(
        DataContext dataContext,
        IKdfService kdfService,
        ISaltService saltService
        ) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Register(UserRegisterFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                Dictionary<string, string> errors = [];
                foreach (var kv in ModelState)
                {
                    string errorMessage = string.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errors[kv.Key] = errorMessage;
                    }
                }


                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }

            return Json(new { Status = "OK" });
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UserUpdateFormModel formModel)
        {
            // Перевіряємо наявність на правильність автентифікації
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                return Unauthorized("No user in context");
            }

            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
            var user = dataContext.Users
                .First(u => u.Id == Guid.Parse(userId))!;

            if (user == null)
            {
                return Unauthorized("Invalid user restoring from identity");
            }

            // Зберігаємо тип сутності (User)
            Type userType = user.GetType();

            // Перевіряємо, що хоча б одне з полів наявне (не всі null)

            bool isAllNulls = true;

            foreach (var prop in formModel.GetType().GetProperties())
            {
                object? val = prop.GetValue(formModel, null);

                if (val != null)
                {
                    isAllNulls = false;

                    if (prop.Name == "Password")
                    {
                        string salt = saltService.GetSalt();
                        user.Salt = salt;
                        user.Dk = kdfService.Dk(formModel.Password!, salt);
                    }
                    else
                    {
                        var userProp = userType.GetProperty(prop.Name);

                        if (userProp != null)
                        {
                            userProp.SetValue(user, val); // а також переносимо усі ненульові значення до сутності (user)
                        }
                        else
                        {
                            return BadRequest($"Form property '{prop.Name}' not found in entity '{userType.Name}'");
                        }
                    }
                }
            }

            if (isAllNulls)
            {
                return BadRequest("No data for update");
            }
            var saveTask = dataContext.SaveChangesAsync();

            AuthSessionMiddleware.SaveAuth(HttpContext, user);
            await saveTask;

            return Accepted();
        }

        public IActionResult Private()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
                var user = dataContext.Users.Include(u => u.Role).First(u => u.Id == Guid.Parse(userId));
                return View(new UserProfileViewModel() { User = user, IsPersonal = true });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Profile()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
                var user = dataContext.Users.Include(u => u.Role).First(u => u.Id == Guid.Parse(userId))!;
                return View(new UserProfileViewModel() { User = user, IsPersonal = true });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Authenticate()
        {
            // RFC 7617  https://datatracker.ietf.org/doc/html/rfc7617
            // Зворотній шлях - вилучення логіна й пароля для перевірки їх
            // Приклад з стандарту -- Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==

            // Серед заголовків обираємо Authorization та перевіряємо на наявність (не-порожність)
            string authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content("Missing Authorization header");
            }


            // перевіряємо, що значення заголовку починається з правильної схеми (Basic )
            // ! кінцевий пробіл є частиною схеми
            string scheme = "Basic ";

            if (!authHeader.StartsWith(scheme))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid Authorization scheme: {scheme}only");
            }


            // виділяємо частину, що іде після схеми та перевірямо на порожність
            string basicCredentials = authHeader[scheme.Length..]; // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            if (basicCredentials.Length <= 3)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid or empty {scheme}Credentials");
            }

            // декодуємо облікові дані з Base64

            string userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(Convert.FromBase64String(basicCredentials));
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid {scheme}Credentials format: {ex.Message}");
            }


            // Розділяємо облікові дані за ":", контролюємо, що поділ здійснюється на 2 частини
            string[] parts = userPass.Split(':', 2); // 2 - макс к-сть частин, ігноруємо ":" у паролі
            if (parts.Length != 2)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid {scheme}user-pass format: missing ':' separator");
            }

            string login = parts[0];
            string password = parts[1];

            // Шукаємо у базі даних користувача за логіном 
            var user = dataContext.Users.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content("Credentials rejected");
            }

            // Перевіряємо пароль шляхом розрахунку DK з використанням паролю, що переданий в облікових даних, та солі, що зберігається в БД у користувача
            // Розрахований результат має збігатися з тим, що наявний у БД

            string dk = kdfService.Dk(password, user.Salt);
            if (dk != user.Dk)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content("Credentials rejected.");
            }

            // якщо автентифікація пройшла успішно, то зберігаємо інформацію у сесії
            AuthSessionMiddleware.SaveAuth(HttpContext, user);

            return NoContent();
        }
    }
}
