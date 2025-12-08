using ASP_PV411.Data.Entities;

namespace ASP_PV411.Models.Home
{
    public class HomeTemplatesViewModel
    {
        public UserRole UserRole { get; set; } = null!;
        public Token Token { get; set; } = null!;
    }
}
