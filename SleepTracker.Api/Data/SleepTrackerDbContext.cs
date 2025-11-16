using Microsoft.EntityFrameworkCore;
using SleepTracker.Api.Models;

namespace SleepTracker.Api.Data;

public class SleepTrackerDbContext : DbContext
{
    public SleepTrackerDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Sleep> Sleeps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sleep>(entity =>
        {
            entity.ToTable("Records");
            entity.Property(s => s.Start).IsRequired();
            entity.Property(s => s.IsDeleted).IsRequired();
            entity.HasQueryFilter(s => !s.IsDeleted);
        });
    }
}
