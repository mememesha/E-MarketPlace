using System;

namespace EM.Dto
{
    public class OfferDto : ICloneable
    {
        public Guid Id { get; set; }
        public decimal CostOfUnit { get; set; }
        public OfferDescriptionDto? OfferDescriptionDto { get; set; }
        public PlaceDto? PlaceDto { get; set; }
        public OrganizationShortDto? OrganizationShortDto { get; set; }

        public object Clone()
        {
            return new OfferDto
            {
                Id = this.Id,
                CostOfUnit = this.CostOfUnit,
                OfferDescriptionDto = (OfferDescriptionDto)OfferDescriptionDto!.Clone(),
                PlaceDto = (PlaceDto)PlaceDto!.Clone(),
                OrganizationShortDto = (OrganizationShortDto)OrganizationShortDto!.Clone()
            };
        }
    }
}