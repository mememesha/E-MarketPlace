using Microsoft.EntityFrameworkCore;
using EM.Contracts;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Newtonsoft.Json;
using EM.MicroService.Offers.Services;

namespace EM.MicroService.Offers.Data;

public sealed class OffersDbContext : DbContext
{
    public DbSet<Offer>? Offers { get; set; }
    public DbSet<OfferDescription>? OfferDescriptions { get; set; }
    public DbSet<Category>? Categories { get; set; }

    public OffersDbContext(DbContextOptions<OffersDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>(
            entity =>
            {
                entity.HasKey(o => o.Id);

                entity.HasOne(o => o.OfferDescription)
                    .WithMany(od => od.Offers)
                    .HasForeignKey(o => o.OfferDescriptionId)
                    .IsRequired();

                entity.Navigation(o => o.OfferDescription)
                    .AutoInclude();

                entity.Ignore(o => o.Place);

                entity.Ignore(o => o.Organization);

                entity.Ignore(o => o.Reserves);
            }
        );

        modelBuilder.Entity<OfferDescription>(
            entity =>
            {
                entity.HasKey(od => od.Id);
                entity.HasOne(od => od.Category)
                    .WithMany(c => c.OfferDescriptions)
                    .HasForeignKey(od => od.CategoryId)
                    .IsRequired();

                entity.Navigation(od => od.Offers)
                    .AutoInclude();

                entity.Navigation(od => od.Category)
                    .AutoInclude();

                entity.Property(od => od.Title)
                    .HasMaxLength(50);

                entity.Property(od => od.Description)
                    .HasMaxLength(5000);
            }
        );

        modelBuilder.Entity<Category>(
            entity =>
            {
                entity.HasKey(od => od.Id);
            }
        );
    }
}
