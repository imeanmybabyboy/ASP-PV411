namespace ASP_PV411.Data.Entities
{
    public class Token
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
