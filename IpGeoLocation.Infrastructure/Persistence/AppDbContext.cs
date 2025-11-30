using IpGeoLocation.Domain.Batches;
using Microsoft.EntityFrameworkCore;

namespace IpGeoLocation.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<BatchItem> BatchItems => Set<BatchItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
