namespace ASP_PV411.Services.Random
{
    public class NewRandomService : IRandomService
    {
        static readonly System.Random _random = new();

        public int RandomInt(int max) => _random.Next();
    }
}
