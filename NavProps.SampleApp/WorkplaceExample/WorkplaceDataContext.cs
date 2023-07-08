using Microsoft.EntityFrameworkCore;

namespace NavProps.SampleApp.WorkplaceExample;

public class WorkplaceDataContext : DbContext
{
    public DbSet<Employee> Employees { get; init; } = default!;
    public DbSet<Office> Offices { get; init; } = default!;
    public DbSet<Department> Departments { get; init; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("WorkplaceDataContext");
    }
}
