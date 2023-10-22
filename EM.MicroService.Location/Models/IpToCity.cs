using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EM.MicroService.Location.Models;

[Keyless]
[Table("ip_city")]
public class IpToCity
{
    [Column("ip_from")]
    public int Ip_from { get; set; }

    [Column("ip_to")]
    public int Ip_to { get; set; }
    
    [Column("country_code")]
    public string? Country_code { get; set; }
    
    [Column("state")]
    public string? State { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("latitude")]
    public float Latitude { get; set; }
    
    [Column("longitude")]
    public float Longitude { get; set; }
}