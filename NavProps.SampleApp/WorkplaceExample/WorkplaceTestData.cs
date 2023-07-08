namespace NavProps.SampleApp.WorkplaceExample;

public static class WorkplaceTestData
{
    public static async Task SeedTestData()
    {
        var dataContext = new WorkplaceDataContext();

        var headOffice = new Office("Head Office", "123 Main Street");
        var secondaryOffice = new Office("Secondary Office", "456 Secondary Avenue");
        dataContext.AddRange(headOffice, secondaryOffice);
        await dataContext.SaveChangesAsync();

        var financeDepartment = new Department("Finance");
        var researchDepartment = new Department("Research");
        dataContext.AddRange(financeDepartment, researchDepartment);
        await dataContext.SaveChangesAsync();

        var headOfficeFinanceEmployee = new Employee("Head Office Finance Employee", headOffice.OfficeId, financeDepartment.DepartmentId);
        var headOfficeResearchEmployee = new Employee("Head Office Research Employee", headOffice.OfficeId, researchDepartment.DepartmentId);

        var secondaryOfficeFinanceEmployee = new Employee("Secondary Office Finance Employee", secondaryOffice.OfficeId, financeDepartment.DepartmentId);
        var secondaryOfficeResearchEmployee = new Employee("Secondary Office Research Employee", secondaryOffice.OfficeId, researchDepartment.DepartmentId);

        var remoteFinanceEmployee = new Employee("Remote Finance Employee", null, financeDepartment.DepartmentId);
        var remoteResearchEmployee = new Employee("Remote Research Employee", null, researchDepartment.DepartmentId);

        dataContext.AddRange(
            headOfficeFinanceEmployee,
            headOfficeResearchEmployee,
            secondaryOfficeFinanceEmployee,
            secondaryOfficeResearchEmployee,
            remoteFinanceEmployee,
            remoteResearchEmployee
        );

        await dataContext.SaveChangesAsync();
    }
}
