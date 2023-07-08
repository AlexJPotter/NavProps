namespace NavProps.SampleApp.WarehouseExample;

public class Order
{
    public string OrderId { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public string CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public string ProductId { get; private set; }
    public Product? Product { get; private set; }

    public int Quantity { get; private set; }

    public Order(string customerId, DateTimeOffset orderDate, string productId, int quantity)
    {
        OrderId = Guid.NewGuid().ToString();
        OrderDate = orderDate;
        CustomerId = customerId;
        ProductId = productId;
        Quantity = quantity;
    }
}
