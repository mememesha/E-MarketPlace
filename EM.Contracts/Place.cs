namespace EM.Contracts
{
    public class Place : EntityBase
    {
        public virtual Guid OrganizationId { get; set; }
        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual GeoTag GeoTag { get; set; }

        public virtual string Address { get; set; }
    }
}