namespace EM.Contracts
{
    public class Sale : EntityBase
    {
        public virtual Guid OrganizationId { get; set; }

        //TODO: думаю, что части продажи могут относиться к разным организациям. 
        public virtual List<SaleBundle> Bundles { get; set; }
    }
}