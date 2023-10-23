namespace EM.Contracts
{
    public class OfferDescription : EntityBase
    {
        public virtual IEnumerable<Offer>? Offers { get; }
        public virtual string? Title { get; set; }
        public virtual string? Description { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        /// <summary> 
        /// true - объявление о продаже
        /// false -  о покупке 
        /// </summary>
        public virtual bool IsSale { get; set; }
        public virtual MetricUnit MetricUnit { get; set; }
        public virtual string? Image { get; set; }
    }

}