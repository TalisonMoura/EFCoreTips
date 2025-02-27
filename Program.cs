using EFCore.Tips.Data;
using EFCore.Tips.Domain;
using System.Diagnostics;
using EFCore.Tips.Interceptors;
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
            //NotUnicode();
            //AgregateOperators();
            //AgregateOperatorsInAgrupment();
            //EventsCounters();
            Listener();
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

        static void AgregateOperators()
        {
            using var db = new ApplicationContext();

            var sql = db.Departments
                .GroupBy(x => x.Description)
                .Select(d => new
                {
                    Description = d.Key,
                    Count = d.Count(),
                    Max = d.Max(a => a.Chair),
                    Sum = d.Sum(a => a.Chair),
                    Average = d.Average(a => a.Chair)
                }).ToQueryString();

            Console.WriteLine($"Genereted Query: {sql}");
        }

        static void AgregateOperatorsInAgrupment()
        {
            using var db = new ApplicationContext();

            var sql = db.Departments
                .GroupBy(x => x.Description)
                .Where(c => c.Count() > 1)
                .Select(d => new
                {
                    Description = d.Key,
                    Count = d.Count(),
                }).ToQueryString();

            Console.WriteLine($"Genereted Query: {sql}");
        }

        // dotnet counters monitor -p {GetCurrentProcessId}.Id --counters Microsoft.EntityFrameworkCore
        static void EventsCounters()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Console.WriteLine($" PID: {Process.GetCurrentProcess().Id}");

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                var department = new Department { Description = "Department without employee" };

                db.Departments.Add(department);
                db.SaveChanges();

                _ = db.Departments.Find(Guid.Empty);
                _ = db.Departments.AsNoTracking().FirstOrDefault();
            }
        }

        static void Listener()
        {
            DiagnosticListener.AllListeners.Subscribe(new MyInterceptorListener());

            using var db = new ApplicationContext();

            _ = db.Departments.Where(x => x.Id == Guid.Empty).ToArray();
        }

    }
}