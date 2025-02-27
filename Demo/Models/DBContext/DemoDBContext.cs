using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EasySignWorkFlow;
using Demo.Enums;

namespace Demo.Models.DBContext;

public class DemoDBContext(DbContextOptions<DemoDBContext> context) : DbContext(context)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRequest>().OwnsOne(x => x.CurrentState);
        modelBuilder.Entity<TestRequest>().OwnsMany(x => x.Statuses);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestRequest> TestRequests { get; set; }
}
