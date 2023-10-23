namespace EM.Contracts
{
    public abstract class EntityBase
    {
        public virtual Guid Id { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTime? DeleteDateTime { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime LastModify { get; set; }
    }
}