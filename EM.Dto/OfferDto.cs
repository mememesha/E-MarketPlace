using System;

namespace EM.Dto
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsSale { get; set; }
        public decimal CostOfUnit { get; set; }
        public string? Image { get; set; }
    }
}