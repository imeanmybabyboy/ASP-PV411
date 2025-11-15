namespace ASP_PV411.Data.Entities
{
    public class UserData
    {
        public string UserId { get; set; } = null!;
        public string? University { get; set; }
        public string? School { get; set; }
        public string? DriverLicence { get; set; }
        public string Nationality { get; set; } = null!;
        public string FirstLang { get; set; } = null!;
    }
}
