namespace ASP_PV411.Services.Signature
{
    public static class SignatureServiceExtension
    {
        public static void AddSignature(this IServiceCollection services)
        {
            services.AddSingleton<ISignatureService, HmacSha2SignatureService>();
        }
    }
}
