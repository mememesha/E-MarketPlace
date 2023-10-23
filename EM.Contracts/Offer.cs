namespace EM.Contracts
{
    public class Offer : EntityBase
    {
        public virtual Guid PlaceId { get; set; }
        public virtual Place? Place { get; set; }
        public virtual Guid OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        public virtual Guid OfferDescriptionId { get; set; }
        public virtual OfferDescription? OfferDescription { get; set; }
        public virtual bool IsInfinitiveResource { get; set; }
        public virtual decimal CostOfUnit { get; set; }
        /// <summary> 
        /// для продажи количество, которое можем продать (например имеется на складах). 
        /// для покупки количество которое собираемся приобрести в этом контракте.
        /// </summary>
        public virtual decimal Quantity { get; set; }
        /// <summary>
        /// для продажи это бронь.
        /// для приобретения - откликнувшиеся, готовые предоставить.
        /// не очень себе представляю, если я размести объявление о перевозке, то например меня могут
        /// две организации забронировать на одно время...
        /// </summary>
        public virtual List<Guid>? ReservesId { get; set; }
        public virtual List<Reserve>? Reserves { get; set; }
        public virtual bool IsArchived { get; set; }
    }
}