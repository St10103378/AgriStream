using Microsoft.EntityFrameworkCore;

namespace AgriStream.Web.Models;

public class AgriStreamDbContext : DbContext
{
    public AgriStreamDbContext(DbContextOptions<AgriStreamDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Device>()
            .HasKey(d => d.DeviceId);
    }
}
