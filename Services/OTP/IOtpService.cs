namespace ASP_PV411.Services.OTP
{
    public interface IOtpService
    {
        string GetOneTimePassword(int? length = null);
    }
}
