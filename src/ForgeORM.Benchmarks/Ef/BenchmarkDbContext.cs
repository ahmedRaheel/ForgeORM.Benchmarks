using ForgeORM.Benchmarks.Models;
using Microsoft.EntityFrameworkCore;

namespace ForgeORM.Benchmarks.Ef;

public sealed class BenchmarkDbContext : DbContext
{
    public BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().ToTable("Customers", "dbo").HasKey(x => x.Id);
        modelBuilder.Entity<Category>().ToTable("Categories", "dbo").HasKey(x => x.Id);
        modelBuilder.Entity<Product>().ToTable("Products", "dbo").HasKey(x => x.Id);
        modelBuilder.Entity<Order>().ToTable("Orders", "dbo").HasKey(x => x.Id);
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems", "dbo").HasKey(x => x.Id);
        modelBuilder.Entity<Payment>().ToTable("Payments", "dbo").HasKey(x => x.Id);

        modelBuilder.Entity<Order>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(x => x.SubTotal).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(x => x.Tax).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(x => x.GrandTotal).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(x => x.LineTotal).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(18, 2);
    }
}
