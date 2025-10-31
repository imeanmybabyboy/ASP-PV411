namespace ASP_PV411.Services.Timestamp
{
    public class EpochTimestampService : ITimestampService
    {
        public long Timestamp()
        {
            return DateTime.Now.Ticks;
        }
    }
}
