using Microsoft.EntityFrameworkCore;
using NavProps.SampleApp.WarehouseExample;
using NavProps.SampleApp.WorkplaceExample;

await WorkplaceTestData.SeedTestData();

var employees =
    await new WorkplaceDataContext().Employees
        .AsNoTracking()
        .Include(e => e.Office)
        .Include(e => e.Department)
        .ToListAsync();

foreach (var employee in employees)
{
    Console.WriteLine($"Employee: {employee.Name}");
    Console.WriteLine($"Office: {employee.NavProps().Office?.Name ?? "None"}");
    Console.WriteLine($"Department: {employee.NavProps().Department.Name}");
    Console.WriteLine();
}

await WarehouseTestData.SeedTestData();

var customers =
    await new WarehouseDataContext().Customers
        .AsNoTracking()
        .Include(c => c.Orders!)
        .ThenInclude(o => o.Product!)
        .ThenInclude(p => p.Supplier)
        .ToListAsync();

foreach (var customer in customers)
{
    Console.WriteLine($"Customer: {customer.Name}");
    foreach (var order in customer.NavProps().Orders)
    {
        Console.WriteLine($"Order: {order.OrderId}");
        Console.WriteLine($"Product: {order.NavProps().Product.Name}");
        Console.WriteLine($"Supplier: {order.NavProps().Product.NavProps().Supplier.Name}");
        Console.WriteLine();
    }
}
