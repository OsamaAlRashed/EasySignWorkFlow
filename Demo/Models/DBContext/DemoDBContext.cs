using EasySignWorkFlow.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Demo.Models.DBContext;

public class DemoDBContext(DbContextOptions<DemoDBContext> context) : DbContext(context)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureRequests(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestRequest> TestRequests { get; set; }
}
