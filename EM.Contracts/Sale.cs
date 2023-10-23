namespace EM.Contracts
{
    public class Sale : EntityBase
    {
        public virtual Guid OrganizationId { get; set; }
        public virtual string? Description { get; set; }
        public virtual List<Guid>? ReserveIds { get; set; }
    }
}