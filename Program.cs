using EFCore.Tips.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Tips
{
    internal class Program
    {
        static void Main(string[] args)
        {
            QueryString();
        }

        static void QueryString()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureCreated();

            var query = db.Departments.Where(x => x.Id != Guid.Empty);

            var generetedSql = query.ToQueryString();

            Console.WriteLine(generetedSql);
        }
    }
}