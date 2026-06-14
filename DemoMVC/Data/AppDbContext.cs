using DemoMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Таблицы
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Manufacturer> Manufacturers { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public DbSet<PickupPoint> PickupPoints { get; set; } = null!;
        public DbSet<Measurement> Measurements { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Администратор" },
                new Role { Id = 2, Name = "Модератор" },
                new Role { Id = 3, Name = "Покупатель" }
            );
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, Name = "Новый" },
                new OrderStatus { Id = 2, Name = "В обработке" },
                new OrderStatus { Id = 3, Name = "Доставлен" }
            );
            modelBuilder.Entity<Measurement>().HasData(
                new Measurement { Id = 1, Name = "шт." }
            );
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Поставщик №1" },
                new Supplier { Id = 2, Name = "Поставщик №2" },
                new Supplier { Id = 3, Name = "Поставщик №3" },
                new Supplier { Id = 4, Name = "Поставщик №4" },
                                new Supplier { Id = 5, Name = "Поставщик №5" }
            );
            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer { Id = 1, Name = "Производитель №1" },
                new Manufacturer { Id = 2, Name = "Производитель №2" },
                new Manufacturer { Id = 3, Name = "Производитель №3" },
                new Manufacturer { Id = 4, Name = "Производитель №4" },
                new Manufacturer { Id = 5, Name = "Производитель №5" }
            );
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Категория №1" },
                new Category { Id = 2, Name = "Категория №2" },
                new Category { Id = 3, Name = "Категория №3" },
                new Category { Id = 4, Name = "Категория №4" },
                new Category { Id = 5, Name = "Категория №5" }
            );
        }
    }
}
