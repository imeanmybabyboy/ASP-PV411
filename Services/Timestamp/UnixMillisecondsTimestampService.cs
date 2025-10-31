namespace ASP_PV411.Services.Timestamp
{
    public class UnixMillisecondsTimestampService : ITimestampService
    {
        public long Timestamp()
        {
            return (DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / 10000;
        }
    }
}
