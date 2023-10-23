namespace EM.Dto;

public class ReserveRequestDto
{
    public Guid OfferId { get; set; }
    public decimal Count { get; set; }
    public Guid ReserveOwnerId { get; set; }
    public DateTime ReleaseDate { get; set; }
}