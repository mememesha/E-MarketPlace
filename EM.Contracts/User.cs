namespace EM.Contracts
{
    public class User : EntityBase
    {
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual List<Contact> Contacts { get; set; }

    }
}