using Microsoft.EntityFrameworkCore;

namespace Demo.Models.DBContext;

public class DemoDBContext : DbContext
{
    public DemoDBContext(DbContextOptions<DemoDBContext> context) : base(context) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // todo
        modelBuilder.Entity<CashRequest>().OwnsMany("CashRequestStatus", x => x.Statuses);
        modelBuilder.Entity<Leave>().OwnsMany("LeaveStatus", x => x.Statuses);
        modelBuilder.Entity<TestRequest>().OwnsMany("TestRequestStatus", x => x.Statuses);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CashRequest> CashRequests { get; set; }
    public DbSet<Leave> Leaves { get; set; }
    public DbSet<TestRequest> TestRequests { get; set; }
}
