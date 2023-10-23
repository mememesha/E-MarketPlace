
namespace EM.Dto
{
    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? OGRN { get; set; }
        public string? INN { get; set; }
        public List<OfferShortResponseDto>? OfferShortResponseDtos { get; set; }
        public List<PlaceDto>? PlaceDtos { get; set; }
    }
}