using Microsoft.EntityFrameworkCore;
using EM.Contracts;

namespace EM.MicroService.Organizations.Data;

public class OrganizationsDbContext : DbContext
{
    public DbSet<Organization>? Organizations { get; set; }

    public OrganizationsDbContext(DbContextOptions<OrganizationsDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(
            entity =>
            {
                entity.HasKey(o => o.Id);

                // entity.Ignore(o => o.Contacts);
                // entity.Ignore(o => o.Users);
                entity.Ignore(o => o.Places);
                entity.Ignore(o => o.Offers);
            }
        );
    }
}