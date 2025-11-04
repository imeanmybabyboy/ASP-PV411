using ASP_PV411.Services.Random;

namespace ASP_PV411.Services.Salt
{
    public static class SaltServiceExtension
    {
        public static void AddSalt(this IServiceCollection services)
        {
            services.AddSingleton<ISaltService, AbcSaltService>();
        }
    }
}
