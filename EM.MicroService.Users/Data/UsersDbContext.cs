using Microsoft.EntityFrameworkCore;
using EM.Contracts;

namespace EM.MicroService.Users.Data;

public class UsersDbContext : DbContext
{
    public DbSet<User>? Users { get; set; }
    public DbSet<UserWithRole>? UsersWithRole { get; set; }

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(
            entity =>
            {
                entity.HasKey(u => u.Id);

                entity.HasMany(u => u.UsersWithRole)
                        .WithOne(uwr => uwr.User)
                        .HasForeignKey(uwr => uwr.UserId);
            }
        );

        modelBuilder.Entity<UserWithRole>(
            entity =>
            {
                entity.HasKey(uwr => uwr.Id);

                entity.Ignore(uwr => uwr.Organization);
                entity.Ignore(uwr => uwr.User);
            }
        );
    }
}