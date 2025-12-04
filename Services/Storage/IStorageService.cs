namespace ASP_PV411.Services.Storage
{
    public interface IStorageService
    {
        byte[] Get(string fileName);
        string Save(IFormFile formFile);
    }
}
