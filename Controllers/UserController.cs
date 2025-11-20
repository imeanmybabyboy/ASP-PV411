using ASP_PV411.Data;
using ASP_PV411.Services.Kdf;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ASP_PV411.Controllers
{
    public class UserController(
        DataContext dataContext,
        IKdfService kdfService) : Controller
    {
        public IActionResult Index()
        {
            return View();
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

            return NoContent();
        }
    }
}
