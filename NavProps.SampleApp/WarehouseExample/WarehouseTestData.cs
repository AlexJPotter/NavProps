namespace NavProps.SampleApp.WarehouseExample;

public static class WarehouseTestData
{
    public static async Task SeedTestData()
    {
        var warehouseDataContext = new WarehouseDataContext();

        var supplier1 = new Supplier("Supplier 1");
        var supplier2 = new Supplier("Supplier 2");
        warehouseDataContext.Suppliers.AddRange(supplier1, supplier2);
        await warehouseDataContext.SaveChangesAsync();

        var product1 = new Product("Product 1", supplier1.SupplierId);
        var product2 = new Product("Product 2", supplier1.SupplierId);
        var product3 = new Product("Product 3", supplier2.SupplierId);
        var product4 = new Product("Product 4", supplier2.SupplierId);
        warehouseDataContext.Products.AddRange(product1, product2, product3, product4);
        await warehouseDataContext.SaveChangesAsync();

        var customer1 = new Customer("Customer 1");
        var customer2 = new Customer("Customer 2");
        warehouseDataContext.Customers.AddRange(customer1, customer2);
        await warehouseDataContext.SaveChangesAsync();

        var order1 = new Order(customer1.CustomerId, DateTimeOffset.UtcNow, product1.ProductId, 10);
        var order2 = new Order(customer1.CustomerId, DateTimeOffset.UtcNow, product2.ProductId, 20);
        var order3 = new Order(customer2.CustomerId, DateTimeOffset.UtcNow, product3.ProductId, 30);
        var order4 = new Order(customer2.CustomerId, DateTimeOffset.UtcNow, product4.ProductId, 40);
        warehouseDataContext.Orders.AddRange(order1, order2, order3, order4);
        await warehouseDataContext.SaveChangesAsync();
    }
}
