namespace ASP_PV411.Services.FolderName
{
    public static class FolderNameExtension
    {
        public static void AddFolderName(this IServiceCollection services)
        {
            services.AddSingleton<IFolderNameService, FolderNameService>();
        }
    }
}
