namespace NavProps.SampleApp.WarehouseExample;

public class Customer
{
    public string CustomerId { get; private set; }

    public string Name { get; private set; }

    public ICollection<Order>? Orders { get; private set; }

    public Customer(string name)
    {
        CustomerId = Guid.NewGuid().ToString();
        Name = name;
    }
}
