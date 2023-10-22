using Microsoft.EntityFrameworkCore;
using EM.MicroService.Location.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EM.MicroService.Location.Data;

public class LocationDbContext : DbContext
{
    public DbSet<IpToCity>? Ip_City { get; set; }

    public LocationDbContext(DbContextOptions<LocationDbContext> dbContextOptions) :
        base(dbContextOptions)
    {
        
    }

}
