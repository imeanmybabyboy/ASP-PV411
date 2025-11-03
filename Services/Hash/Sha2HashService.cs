namespace ASP_PV411.Services.Hash
{
    public class Sha2HashService : IHashService
    {
        public string Digest(string input)
        {
            return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(input)));
        }

    }
}
