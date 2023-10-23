using Microsoft.EntityFrameworkCore;
using EM.Contracts;

namespace EM.MicroService.Location.Data;

public class LocationDbContext : DbContext
{
    public DbSet<Place>? Places { get; set; }

    public DbSet<GeoTag>? GeoTags { get; set; }

    public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options)
    {
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Place>(
            entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.GeoTag)
                    .WithMany(gt => gt.Places);

                entity.Ignore(p => p.Offer);

                entity.Ignore(p => p.Organization);
            }
        );

        modelBuilder.Entity<GeoTag>(
            entity =>
            {
                entity.HasKey(gt => gt.Id);

                entity.Ignore(gt => gt.Places);
            }
        );
    }
}
