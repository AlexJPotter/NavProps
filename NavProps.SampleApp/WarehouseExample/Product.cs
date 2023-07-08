namespace NavProps.SampleApp.WarehouseExample;

public class Product
{
    public string ProductId { get; private set; }

    public string Name { get; private set; }

    public string SupplierId { get; private set; }
    public Supplier? Supplier { get; private set; }

    public ICollection<Order>? Orders { get; private set; }

    public Product(string name, string supplierId)
    {
        ProductId = Guid.NewGuid().ToString();
        Name = name;
        SupplierId = supplierId;
    }
}
