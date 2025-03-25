using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EasySignWorkFlow;
using Demo.Enums;

namespace Demo.Models.DBContext;

public class DemoDBContext(DbContextOptions<DemoDBContext> context) : DbContext(context)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRequest>().OwnsOne("CurrentState", x => x.CurrentState);
        modelBuilder.Entity<TestRequest>().OwnsMany("TestRequestStates", x => x.States);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestRequest> TestRequests { get; set; }
}
