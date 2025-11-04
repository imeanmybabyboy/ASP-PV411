namespace ASP_PV411.Services.Random
{
    public class RandomService : IRandomService
    {
        static readonly System.Random _random = new();

        public int RandomInt(int max) => _random.Next(max);

    }
}
