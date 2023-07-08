using Microsoft.EntityFrameworkCore;

namespace NavProps.SampleApp.WarehouseExample;

public class WarehouseDataContext : DbContext
{
    public DbSet<Customer> Customers { get; init; } = default!;
    public DbSet<Order> Orders { get; init; } = default!;
    public DbSet<Product> Products { get; init; } = default!;
    public DbSet<Supplier> Suppliers { get; init; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("WarehouseDataContext");
    }
}
