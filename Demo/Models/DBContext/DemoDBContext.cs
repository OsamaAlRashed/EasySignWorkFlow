using Microsoft.EntityFrameworkCore;

namespace Demo.Models.DBContext;

public class DemoDBContext : DbContext
{
    public DemoDBContext(DbContextOptions<DemoDBContext> context) : base(context) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // todo
        modelBuilder.Entity<CashRequest>().OwnsMany("CashRequestStatus", "_statuses");
        modelBuilder.Entity<Leave>().OwnsMany("LeaveStatus", "_statuses");

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<CashRequest> CashRequests { get; set; }
    public DbSet<Leave> Leaves { get; set; }
}
