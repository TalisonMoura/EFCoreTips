using EFCore.Tips.Data;
using EFCore.Tips.Domain;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Tips
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //QueryString();
            //ClearContext();
            //ToView();
            NotUnicode();
        }

        static void QueryString()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureCreated();

            var query = db.Departments.Where(x => x.Id != Guid.Empty);

            var generetedSql = query.ToQueryString();

            Console.WriteLine(generetedSql);
        }

        static void ClearContext()
        {
            using var db = new ApplicationContext();

            db.Departments.Add(new() { Description = "Tecnology changes the wolrd" });

            db.ChangeTracker.Clear();
        }

        static void ToView()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Database.ExecuteSqlRaw(
                @"CREATE VIEW vw_report_department AS
                  SELECT
                    d.Description, count(e.Id) as Employees
                  FROM
                    Departments d
                  LEFT JOIN Employees e ON e.DepartmentId = d.Id
                  GROUP BY d.Description");

            var departments = Enumerable.Range(1, 10)
                .Select(x => new Department
                {
                    Description = $"Department {x}",
                    Employees = Enumerable.Range(1, x)
                    .Select(e => new Employee
                    {
                        Name = $"Employee {x}-{e}"
                    }).ToList()
                });

            var department = new Department { Description = "Department without employee" };

            db.Departments.Add(department);
            db.Departments.AddRange(departments);
            db.SaveChanges();

            var report = db.ReportDepartments
                           .Where(x => x.Employees < 20)
                           .OrderBy(x => x.Department)
                           .ToList();

            foreach (var item in report)
                Console.WriteLine($"{item.Department} [ Employees: {item.Employees}]");
        }

        static void NotUnicode()
        {
            using var db = new ApplicationContext();

            var sql = db.Database.GenerateCreateScript();

            Console.WriteLine($"Genereted Scrípt: {sql}");
        }
    }
}