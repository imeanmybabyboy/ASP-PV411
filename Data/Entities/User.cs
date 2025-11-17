namespace ASP_PV411.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string RoleId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly? Birthdate { get; set; }
        public string Login { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string Dk { get; set; } = null!;
        public DateTime RegisterAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public UserRole Role { get; set; } = null!;
    }
}
