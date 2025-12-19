using ASP_PV411.Data.Entities;
using ASP_PV411.Services.Kdf;
using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Data
{
    public class DataContext : DbContext
    {
        // В ASP прийнято конфігурувати контекст як сервіс (у Program.cs), тому переозначається конструктор, що приймає параметр-конфігуратор

        private readonly IKdfService _kdfService;

        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.UserRole> UserRoles { get; set; }
        public DbSet<Entities.Token> Tokens { get; set; }
        public DbSet<Entities.Group> Groups { get; set; }
        public DbSet<Entities.Manufacturer> Manufacturers { get; set; }
        public DbSet<Entities.Product> Products { get; set; }
        public DbSet<Entities.Cart> Carts { get; set; }
        public DbSet<Entities.CartItem> CartItems { get; set; }
        public DbSet<Entities.Discount> Discounts { get; set; }

        public DataContext(DbContextOptions options, IKdfService kdfService) : base(options)
        {
            _kdfService = kdfService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>()
                .HasIndex(u => u.Login) // індекс - інформація про впорядкування
                .IsUnique();


            modelBuilder.Entity<Entities.User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Entities.Token>()
                .HasOne(t => t.User)
                .WithMany();

            modelBuilder.Entity<Entities.CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems);
            
            modelBuilder.Entity<Entities.CartItem>()
                .HasOne(ci => ci.Discount)
                .WithMany();

            modelBuilder.Entity<Entities.Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts);

            modelBuilder.Entity<Entities.Cart>()
                .HasOne(c => c.Discount)
                .WithMany();

            modelBuilder.Entity<Entities.CartItem>()
                .HasOne(ci => ci.Discount)
                .WithMany();


            modelBuilder.Entity<Entities.Cart>()
                .HasOne(c => c.Discount)
                .WithMany();



            ///////////// Сідування 
            modelBuilder.Entity<Entities.UserRole>()
                .HasData(
                    new Entities.UserRole
                    {
                        Id = "Admin",
                        Description = "Full access role",
                        CanCreate = 1,
                        CanDelete = 1,
                        CanRead = 1,
                        CanUpdate = 1
                    },

                    new Entities.UserRole
                    {
                        Id = "User",
                        Description = "Self registered user",
                        CanCreate = 0,
                        CanDelete = 0,
                        CanRead = 0,
                        CanUpdate = 0
                    }
                );

            string salt = "F94049E318A2";
            string dk = _kdfService.Dk("Admin", salt);

            string userSalt = "5875D913075F";
            string userDk = _kdfService.Dk("user", userSalt);

            modelBuilder.Entity<Entities.User>()
                .HasData(
                    new Entities.User
                    {
                        Id = Guid.Parse("85FE8311-DF9C-4322-B093-F94049E318A2"),
                        Name = "Administrator",
                        Email = "admin@change.me",
                        Salt = salt,
                        Login = "Admin",
                        Dk = dk,
                        RoleId = "Admin",
                        RegisterAt = DateTime.MinValue,
                    },

                    new Entities.User
                    {
                        Id = Guid.Parse("5588B536-186B-436C-AA1A-5875D913075F"),
                        Name = "user",
                        Email = "newUser@example.com",
                        Salt = userSalt,
                        Login = "User",
                        Dk = userDk,
                        RoleId = "User",
                        RegisterAt = DateTime.MinValue,
                    }
                );

            modelBuilder.Entity<Entities.Product>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Products);
        }
    }
}


/*
 Shop:              
[Group]            [Product]         [Manufacturer]
 Id                 Id                Id         
 Name               GroupId           Name       
 Description        ManufacturerId    Description
 ImageUrl           Name              ImageUrl   
 DeleteAt           Description       DeleteAt
                    Price
                    Stock
                    ImageUrl
                    DeleteAt


 */