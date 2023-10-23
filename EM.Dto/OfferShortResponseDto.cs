using System;

namespace EM.Dto
{
    public class OfferShortResponseDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public decimal CostOfUnit { get; set; }
        public string? Image { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}