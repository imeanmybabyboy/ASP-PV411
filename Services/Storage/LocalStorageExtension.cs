using ASP_PV411.Services.Signature;

namespace ASP_PV411.Services.Storage
{
    public static class LocalStorageExtension
    {
        public static void AddStorage(this IServiceCollection services)
        {
            services.AddSingleton<IStorageService, LocalStorageService>();
        }
    }
}
