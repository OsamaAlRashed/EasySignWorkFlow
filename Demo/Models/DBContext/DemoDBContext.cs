using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EasySignWorkFlow;
using Demo.Enums;

namespace Demo.Models.DBContext;

public class DemoDBContext(DbContextOptions<DemoDBContext> context) : DbContext(context)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureRequest<TestRequest, Guid, TestStatus>();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestRequest> TestRequests { get; set; }
}
