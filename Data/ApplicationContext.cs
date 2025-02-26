using EFCore.Tips.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Tips.Data;

public class ApplicationContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<ReportDepartment> ReportDepartments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=TALISONJM\\SQLEXPRESS;Database=EFCoreTips;Integrated Security=true;TrustServerCertificate=True;pooling=true")
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReportDepartment>(rd =>
        {
            rd.HasNoKey();

            rd.ToView("vw_report_department");

            rd.Property(x => x.Department).HasColumnName("Description");
        });

        var properties = modelBuilder.Model.GetEntityTypes()
                                           .SelectMany(x => x.GetProperties())
                                           .Where(x => x.ClrType.Equals(typeof(string)) && x.GetColumnType() is null);
        
        foreach (var property in properties)
            property.SetIsUnicode(false);
    }
}