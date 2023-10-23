namespace EM.Contracts
{
    public class Organization : EntityBase
    {
        public virtual string? Name { get; set; }
        public virtual string? OGRN { get; set; }
        public virtual string? INN { get; set; }
        // public virtual List<Guid>? ContactsId { get; set; }
        // public virtual List<Contact>? Contacts { get; set; }
        public virtual List<Guid>? UsersId { get; set; }
        public virtual List<UserWithRole>? Users { get; set; }
        public virtual List<Guid>? PlacesId { get; set; }
        public virtual List<Place>? Places { get; set; }
        public virtual List<Guid>? OffersId { get; set; }
        public virtual List<Offer>? Offers { get; set; }
        // public virtual List<Sale>? Sales { get; set; }
        //TODO Добавить реквизиты организации для совершения платежей
    }
}