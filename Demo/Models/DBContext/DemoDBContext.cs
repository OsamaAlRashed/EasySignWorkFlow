using Demo.Enums;
using Microsoft.EntityFrameworkCore;

namespace Demo.Models.DBContext;

public class DemoDBContext : DbContext
{
    public DemoDBContext(DbContextOptions<DemoDBContext> context) : base(context) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRequest>().OwnsMany("TestRequestStatus", x => x.Statuses);

        modelBuilder.Entity<TestRequest>()
            .Property(x => x.CurrentStatus)
            .HasComputedColumnSql("select top(1) status from dbo.TestRequests order by DateSigned desc");

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestRequest> TestRequests { get; set; }
}
