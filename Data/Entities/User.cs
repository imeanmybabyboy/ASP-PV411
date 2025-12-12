using Microsoft.AspNetCore.Identity;

namespace ASP_PV411.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string RoleId { get; set; } = null!;

        [PersonalData]
        public string Name { get; set; } = null!;

        [PersonalData]
        public string Email { get; set; } = null!;

        [PersonalData]
        public string? Phone { get; set; } = null!;

        [PersonalData]
        public DateOnly? Birthdate { get; set; }

        public string Login { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string Dk { get; set; } = null!;
        public DateTime RegisterAt { get; set; }
        public DateTime? DeleteAt { get; set; }
            
        public UserRole Role { get; set; } = null!;
        public ICollection<Cart> Carts { get; set; } = [];
    }
}
