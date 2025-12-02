using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace ASP_PV411.Middleware
{
    public class AuthSessionMiddleware
    {
        public const string SessionKey = "AuthSessionMiddleware";
        private readonly RequestDelegate _next;

        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // перевіряємо, чи є запит на вихід з авторизованого режиму
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove(SessionKey);
                context.Response.Redirect(context.Request.Path);
                return; // _next - не буде виконуватися, робота перерветься
            }

            // перевіряємо наявність у сесії збережених даних щодо автентифікації
            if (context.Session.Keys.Contains(SessionKey))
            {
                var user = JsonSerializer.Deserialize<Data.Entities.User>(context.Session.GetString(SessionKey)!)!;

                //context.Items[SessionKey] = user;

                /* Використання (поширення) типу даних з сутностей БД має ряд недоліків:
                 * - зчепленн модулів (шарів) - залежність другого модуля від типів даних першого модуля
                 * - складність відокремлення модуля як сервісу, оскільки модуль, що залишається, втрачає зв'язок з потрібним типом
                 * - складність використання сторонніх сервісів автентифікації (Гугл, ФБ тощо) - вони передають свої дані, можливо, несумісні з нашими типами даних
                 * = Слід використовувати уніфікований інтерфейс, відокремлений від типізації - пари ключ-значення 
                 */

                context.User = new ClaimsPrincipal(                         // Слід використовувати уніфікований
                    new ClaimsIdentity(                                     // інтерфейс відокремлений від
                            [                                               // типізації - пари ключ-значення
                                new Claim(ClaimTypes.Name,                  // Claim(тип, значення)
                                    user.Name),                             //
                                                                            //
                        new Claim(ClaimTypes.Email,                         // Вони поєднуються до
                                    user.Email),                            // ClaimsIdentity, до якої додається
                                                                            // тип автентифікації
                        new Claim(ClaimTypes.DateOfBirth,                   // nameof(AuthSessionMiddleware)
                                    user.Birthdate?.ToString() ?? ""),      // за яким можна дізнатися
                                                                            // походження Claims
                        new Claim(ClaimTypes.NameIdentifier,                //
                                    user.Login),                            // Технічно, користувач може мати
                        new Claim(ClaimTypes.Sid, user.Id.ToString())       // декілька Identity різного
                            ],                                              // походження
                            nameof(AuthSessionMiddleware)                   //
                        )
                    );
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

        public static void SaveAuth(HttpContext context, Data.Entities.User user)
        {
            context.Session.SetString(AuthSessionMiddleware.SessionKey, JsonSerializer.Serialize(user));
        }
        
        public static void Logout(HttpContext context)
        {
            context.Session.Remove(AuthSessionMiddleware.SessionKey);
        }
    }

    public static class AuthSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthSession(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthSessionMiddleware>();
        }
    }
}
