using Microsoft.EntityFrameworkCore;
using EM.Contracts;

namespace EM.MicroService.Reserves.Data;

public sealed class ReservesDbContext : DbContext
{
    public DbSet<Reserve>? Reserves { get; set; }

    public ReservesDbContext(DbContextOptions<ReservesDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reserve>(
            entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Ignore(r => r.Offer);

                entity.Ignore(r => r.ReserveOwner);
            }
        );
    }
}
