namespace NavProps.SampleApp.WarehouseExample;

public class Supplier
{
    public string SupplierId { get; private set; }

    public string Name { get; private set; }

    public ICollection<Product>? Products { get; private set; }

    public Supplier(string name)
    {
        SupplierId = Guid.NewGuid().ToString();
        Name = name;
    }
}
