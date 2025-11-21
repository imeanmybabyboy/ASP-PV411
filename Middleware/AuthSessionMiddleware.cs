using System.Globalization;
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

                context.Items[SessionKey] = user;
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

        public static void SaveAuth(HttpContext context, Data.Entities.User user)
        {
            context.Session.SetString(AuthSessionMiddleware.SessionKey, JsonSerializer.Serialize(user));
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
