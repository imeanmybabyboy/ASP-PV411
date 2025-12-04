namespace ASP_PV411.Services.FileExtensionChecker
{
    public static class FileExtensionCheckerExtension
    {
        public static void AddChecker(this IServiceCollection services)
        {
            services.AddSingleton<IFileExtensionCheckerService, FileExtensionCheckerService>();
        }
    }
}
