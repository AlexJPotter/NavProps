namespace NavProps.SampleApp.WorkplaceExample;

public class Office
{
    public int OfficeId { get; private set; }

    public string Name { get; private set; }
    public string Address { get; private set; }

    public ICollection<Employee>? Employees { get; private set; }

    public Office(string name, string address)
    {
        OfficeId = default;
        Name = name;
        Address = address;
    }
}
