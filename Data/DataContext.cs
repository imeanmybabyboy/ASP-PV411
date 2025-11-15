using ASP_PV411.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Data
{
    public class DataContext : DbContext
    {
        // В ASP прийнято конфігурувати контекст як сервіс (у Program.cs), тому переозначається конструктор, що приймає параметр-конфігуратор

        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.UserRole> UserRoles { get; set; }
        public DbSet<Entities.Token> Tokens { get; set; }
        public DbSet<Entities.UserData> UserDatas { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }
    }
}
