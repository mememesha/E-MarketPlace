namespace EM.Contracts
{
    public class SaleBundle : EntityBase
    {
        public virtual string Description { get; set; }
        public virtual List<Guid> ReserveIds { get; set; }
        public virtual List<Place> Places { get; set; }


    }
}