namespace ASP_PV411.Services.Kdf
{
    public interface IKdfService
    {
        /// <summary>
        /// Derived key from password and salt
        /// </summary>
        string Dk(string password, string salt);
    }
}
