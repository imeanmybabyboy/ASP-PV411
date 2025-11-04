using ASP_PV411.Services.Random;

namespace ASP_PV411.Services.OTP
{
    /// <summary>
    /// gets One Time Password with exact length (default - 6)
    /// </summary>

    public class OtpService(IRandomService randomService) : IOtpService
    {
        private readonly IRandomService _randomService = randomService;

        public string GetOneTimePassword(int? length = null)
        {
            length ??= 6;
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                otp += _randomService.RandomInt(10).ToString();
            }

            return otp;
        }
    }
}
