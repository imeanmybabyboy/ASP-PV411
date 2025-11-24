namespace ASP_PV411.Models.User
{
    public class UserProfileViewModel
    {
        public Data.Entities.User User { get; set; } = null!;
        public bool IsPersonal { get; set; }
    }
}
