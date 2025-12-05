using ASP_PV411.Data.Entities;

namespace ASP_PV411.Models.Admin
{
    public class AdminIndexViewModel
    {
        public List<Manufacturer> Manufacturers { get; set; } = [];
        public List<Group> Groups { get; set; } = [];

    }
}
