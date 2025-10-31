namespace ASP_PV411.Services.Timestamp
{
    public static class TimestampServiceExtension
    {
        public static void AddTimestamp(this IServiceCollection services)
        {
            services.AddSingleton<ITimestampService, EpochTimestampService>();
        }
    }
}
