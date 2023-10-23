namespace EM.Contracts
{
    public class Reserve : EntityBase
    {
        public virtual Guid OfferId { get; set; }
        public virtual Offer? Offer { get; set; }
        public virtual Guid ReserveOwnerId { get; set; }
        public virtual Organization? ReserveOwner { get; set; }
        public virtual DateTime ReleaseDate { get; set; }
        public virtual decimal Count { get; set; }
        public virtual bool IsApproved { get; set; }
    }
}