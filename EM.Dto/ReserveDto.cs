namespace EM.Dto;

public class ReserveDto
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public decimal Count { get; set; }
    public Guid ReserveOwnerId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool IsApproved { get; set; }
}