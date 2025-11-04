namespace ASP_PV411.Services.OTP
{
    public static class OtpServiceExtension
    {
        public static void AddOtp(this IServiceCollection services)
        {
            services.AddSingleton<IOtpService, OtpService>();
        }
    }
}
