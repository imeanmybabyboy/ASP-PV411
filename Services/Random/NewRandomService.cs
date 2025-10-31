namespace ASP_PV411.Services.Random
{
    public class NewRandomService : IRandomService
    {
        static readonly System.Random _random = new();

        public int RandomInt() => _random.Next(0, 1000);

    }
}
