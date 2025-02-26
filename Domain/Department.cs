namespace EFCore.Tips.Domain;

public class Department
{
    public Guid Id { get; set; }
    public int Chair { get; set; }
    public string Description { get; set; }

    public ICollection<Employee> Employees { get; set; }
}
