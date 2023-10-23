namespace EM.Contracts
{
    public class UserWithRole : EntityBase
    {
        public virtual Guid OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
        public virtual UserAbility Role { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual User? User { get; set; }
    }
}