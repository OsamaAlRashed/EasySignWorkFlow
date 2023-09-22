using Microsoft.EntityFrameworkCore;

namespace Demo.Models.DBContext;

public class DemoDBContext : DbContext
{
    public DemoDBContext(DbContextOptions<DemoDBContext> context) : base(context) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRequest>().OwnsMany("TestRequestStatus", x => x.Statuses);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestRequest> TestRequests { get; set; }
}
