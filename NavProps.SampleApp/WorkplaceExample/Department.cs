namespace NavProps.SampleApp.WorkplaceExample;

public class Department
{
    public int DepartmentId { get; private set; }

    public string Name { get; private set; }

    public ICollection<Employee>? Employees { get; private set; }

    public int? OfficeId { get; private set; }
    public Office? Office { get; private set; }


    public Department(string name)
    {
        DepartmentId = default;
        Name = name;
    }
}
