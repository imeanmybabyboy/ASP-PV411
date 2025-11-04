namespace ASP_PV411.Services.Signature
{
    /// <summary>
    /// сервіс цифрового підпису
    /// </summary>
    public interface ISignatureService
    {
        string Sign(string data, string password);
    }
}
