namespace NavProps.SampleApp.WorkplaceExample;

public class Employee
{
    public int EmployeeId { get; private set; }

    public string Name { get; private set; }

    // An Employee does not necessarily have an associated Office (e.g. if they are a remote worker)
    public int? OfficeId { get; private set; }
    public Office? Office { get; private set; }

    public int DepartmentId { get; private set; }
    public Department? Department { get; private set; }

    public Employee(string name, int? officeId, int departmentId)
    {
        EmployeeId = default;
        Name = name;
        OfficeId = officeId;
        DepartmentId = departmentId;
    }
}
