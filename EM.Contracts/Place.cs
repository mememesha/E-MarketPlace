namespace EM.Contracts
{
    public class Place : EntityBase
    {
        public virtual Guid? OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        public virtual string? Title { get; set; }
        public virtual string? Description { get; set; }
        public virtual GeoTag? GeoTag { get; set; }
        public virtual string? Region { get; set; }
        public virtual string? City { get; set; }
        public virtual string? Address { get; set; }
        public virtual Guid? OfferId { get; set; }
        public virtual Offer? Offer { get; set; }
    }
}