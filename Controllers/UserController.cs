using ASP_PV411.Data;
using ASP_PV411.Middleware;
using ASP_PV411.Models.Rest;
using ASP_PV411.Models.User;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.Salt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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

        [HttpDelete]
        public async Task<JsonResult> Erase()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
                var user = dataContext.
                    Users
                    .First(u => u.Id == Guid.Parse(userId));

                //foreach (var prop in user.GetType().GetProperties())
                //{
                //    if (prop.GetCustomAttribute<PersonalDataAttribute>() != null)
                //    {
                //        if (prop.GetType().IsAbstract)
                //        {

                //        }
                //    }
                //}

                user.Name = user.Email = user.Phone = "";
                user.Birthdate = null;
                user.DeleteAt = DateTime.Now;
                var saveTask = dataContext.SaveChangesAsync();

                AuthSessionMiddleware.Logout(HttpContext);

                await saveTask;

                return Json(new { Status = "Ok" });
            }
            else
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new
                {
                    Status = "Error",
                    Error = new Dictionary<string, string>()
                    {
                        {"auth", "User not authorized"}
                    }
                });
            }
        }

        public ViewResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Register(UserRegisterFormModel formModel)
        {
            // валідація паролю
            if (!string.IsNullOrEmpty(formModel.UserPassword))
            {
                if (!Regex.IsMatch(formModel.UserPassword, @"\d"))
                {
                    ModelState.AddModelError("user-password", "Пароль повинен містити принаймні одну цифру");
                }
                else if (!Regex.IsMatch(formModel.UserPassword, @"\W"))
                {
                    ModelState.AddModelError("user-password", "Пароль повинен містити принаймні один спецсимвол");
                }
                else if (!Regex.IsMatch(formModel.UserPassword, @"[a-z]"))
                {
                    ModelState.AddModelError("user-password", "Пароль повинен містити принаймні одну малу літеру");
                }
                else if (!Regex.IsMatch(formModel.UserPassword, @"[A-Z]"))
                {
                    ModelState.AddModelError("user-password", "Пароль повинен містити принаймні одну велику літеру");
                }
            }

            // валідація e-mail
            if (!string.IsNullOrEmpty(formModel.UserEmail))
            {
                if (!Regex.IsMatch(formModel.UserEmail, @"([\w]+)@([\w]+).com"))
                {
                    ModelState.AddModelError("user-email", "Пароль має бути створений за патерном: johnDoe@example.com");
                }
            }

            // Валідація паролю
            if (!string.IsNullOrEmpty(formModel.UserPhone))
            {
                if (!Regex.IsMatch(formModel.UserPhone, @"\+380+"))
                {
                    ModelState.AddModelError("user-phone-number", @"Номер телефону повинен починатися з '+380'");
                }
                else if (formModel.UserPhone.Length != 13)
                {
                    ModelState.AddModelError("user-phone-number", @"Довжина номера телефону має становити 9 символів після +380");
                }
                else if (!Regex.IsMatch(formModel.UserPhone, @"^\+?[0-9]+$"))
                {
                    ModelState.AddModelError("user-phone-number", @"Номер телефону повинен містити лише цифри");
                }
            }

            // Валідація дати народження
            if (formModel!.UserBirthdate != null)
            {
                if (DateOnly.FromDateTime(DateTime.Today).DayNumber - formModel.UserBirthdate?.DayNumber < 16 * 365)
                {
                    ModelState.AddModelError("user-birthdate", "Самостійна реєстрація дозволена з 16 років!");
                }
            }

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

            if (dataContext.Users.Any(u => u.Login == formModel.UserLogin))
            {
                return Json(new
                {
                    Status = "Error",
                    Errors = new Dictionary<string, string>()
                    {
                        { "user-login", "Login in use" }
                    }
                });
            }

            string salt = saltService.GetSalt();
            dataContext.Users.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = formModel.UserName,
                Email = formModel.UserEmail,
                Phone = formModel.UserPhone,
                Login = formModel.UserLogin,
                Salt = salt,
                Dk = kdfService.Dk(formModel.UserPassword, salt),
                Birthdate = formModel.UserBirthdate,
                RegisterAt = DateTime.Now,
                RoleId = "User",
            });
            dataContext.SaveChanges();

            return Json(new { Status = "Ok" });
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
                    else if (prop.Name == "Birthdate")
                    {
                        var userProp = userType.GetProperty(prop.Name);
                        if (userProp != null)
                        {
                            var cleanedBirthdate = formModel.Birthdate?.Trim().Replace("\n", "").Replace("\r", "");

                            if (string.IsNullOrWhiteSpace(cleanedBirthdate))
                            {
                                userProp.SetValue(user, null);
                            }
                            else if (DateOnly.TryParse(cleanedBirthdate, out var date))
                            {
                                userProp.SetValue(user, date);
                            }
                            else
                            {
                                return BadRequest($"Неправильний формат дати народження:({formModel.Birthdate})!");
                            }
                        }
                        else
                        {
                            return BadRequest($"Form property '{prop.Name}' not found in entity '{userType.Name}'");
                        }
                    }
                    else if (prop.Name == "Phone")
                    {
                        var userProp = userType.GetProperty(prop.Name);
                        if (userProp != null)
                        {
                            if (!Regex.IsMatch(formModel.Phone!, @"\+380+") && formModel.Phone != "")
                            {
                                return BadRequest($"Номер телефону повинен починатися з '+380'");
                            }
                            else if (!Regex.IsMatch(formModel.Phone!, @"^\+?[0-9]+$") && formModel.Phone != "")
                            {
                                return BadRequest($"Номер телефону повинен містити лише цифри");
                            }
                            else if (formModel.Phone!.Length != 13 && formModel.Phone != "")
                            {
                                return BadRequest($"Довжина номера телефону має становити 9 символів після +380");
                            }
                            else
                            {
                                if (formModel.Phone == "")
                                {
                                    userProp.SetValue(user, null);
                                }
                                else
                                {
                                    userProp.SetValue(user, val);
                                }
                            }
                        }
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
                Guid userGuid = Guid.Parse(userId);

                var user = dataContext.Users.Include(u => u.Role).First(u => u.Id == userGuid && u.DeleteAt == null)!;
                return View(new UserProfileViewModel()
                {
                    User = user,
                    IsPersonal = true,
                    Carts = dataContext.Carts
                    .OrderByDescending(c => c.OpenAt)
                    .Where(c => c.UserId == userGuid)
                    .ToList(),
                });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private Data.Entities.User _Authenticate()
        {
            // RFC 7617  https://datatracker.ietf.org/doc/html/rfc7617
            // Зворотній шлях - вилучення логіна й пароля для перевірки їх
            // Приклад з стандарту -- Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==

            // Серед заголовків обираємо Authorization та перевіряємо на наявність (не-порожність)
            string authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                throw new Exception("Missing Authorization header");
            }


            // перевіряємо, що значення заголовку починається з правильної схеми (Basic )
            // ! кінцевий пробіл є частиною схеми
            string scheme = "Basic ";

            if (!authHeader.StartsWith(scheme))
            {
                throw new Exception($"Invalid Authorization scheme: {scheme}only");
            }


            // виділяємо частину, що іде після схеми та перевірямо на порожність
            string basicCredentials = authHeader[scheme.Length..]; // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            if (basicCredentials.Length <= 3)
            {
                throw new Exception($"Invalid or empty {scheme}Credentials");
            }

            // декодуємо облікові дані з Base64

            string userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(Convert.FromBase64String(basicCredentials));
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid {scheme}Credentials format: {ex.Message}");
            }


            // Розділяємо облікові дані за ":", контролюємо, що поділ здійснюється на 2 частини
            string[] parts = userPass.Split(':', 2); // 2 - макс к-сть частин, ігноруємо ":" у паролі
            if (parts.Length != 2)
            {
                throw new Exception($"Invalid {scheme}user-pass format: missing ':' separator");
            }

            string login = parts[0];
            string password = parts[1];

            // Шукаємо у базі даних користувача за логіном 
            var user = dataContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == login && u.DeleteAt == null);
            if (user == null)
            {
                throw new Exception("Credentials rejected");
            }

            // Перевіряємо пароль шляхом розрахунку DK з використанням паролю, що переданий в облікових даних, та солі, що зберігається в БД у користувача
            // Розрахований результат має збігатися з тим, що наявний у БД

            string dk = kdfService.Dk(password, user.Salt);
            if (dk != user.Dk)
            {
                throw new Exception("Credentials rejected.");
            }

            return user;
        }

        public IActionResult Authenticate()
        {
            try
            {
                // якщо автентифікація пройшла успішно, то зберігаємо інформацію у сесії
                AuthSessionMiddleware.SaveAuth(HttpContext, _Authenticate());
                return NoContent();
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content(ex.Message);
            }
        }

        public JsonResult ApiAuthenticate()
        {
                RestResponse restResponse = new()
                {
                    Meta = new()
                    {
                        Service = "Користувачі",
                        ServerTime = DateTime.Now.Ticks,
                        Cache = 86400,
                        DataType = "array",
                        Method = Request.Method,
                        Path = Request.Path,
                        Resouce = "Користувачі",
                    },
                };

            try
            {
                restResponse.Data = _Authenticate();

                return Json(_Authenticate());
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(ex.Message);
            }
        }
    }
}
