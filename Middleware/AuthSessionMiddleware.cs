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
            // перевіряємо наявність у сесії збережених даних щодо автентифікації
            if (context.Session.Keys.Contains(SessionKey))
            {
                var user = JsonSerializer.Deserialize<Data.Entities.User>(context.Session.GetString(SessionKey)!)!;

                context.Items[SessionKey] = user;
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
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
