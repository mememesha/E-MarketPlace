namespace EM.Contracts
{
    public class OfferDescription : EntityBase
    {
        public virtual Guid OrganizationId { get; set; }

        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual List<string> Tags { get; set; }


        /// <summary> объявление о продаже или о покупке </summary>
        public virtual bool IsSale { get; set; }

        public virtual MetricUnit MetricUnit { get; set; }
    }

}