using Microsoft.EntityFrameworkCore;
using BookShop.Data.Extensions;
using BookShop.Data.Models;

namespace BookShop.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Order_items> Order_Items { get; set; }
        public DbSet<Category_requests> Category_Requests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Products
            modelBuilder.Entity<Products>()
            .HasOne(p => p.Category)
            .WithMany(b => b.products)
            .HasForeignKey(p => p.category_id);

            // Orders
            modelBuilder.Entity<Orders>()
            .HasOne(p => p.User)
            .WithMany(b => b.orders)
            .HasForeignKey(p => p.user_id);

            // Order_items
            modelBuilder.Entity<Order_items>()
            .HasOne(p => p.Order)
            .WithMany(b => b.order_Items)
            .HasForeignKey(p => p.order_id);
            modelBuilder.Entity<Order_items>()
            .HasOne(p => p.Product)
            .WithMany(b => b.order_Items)
            .HasForeignKey(p => p.product_id);

            modelBuilder.Seed();
        }
    }
}
