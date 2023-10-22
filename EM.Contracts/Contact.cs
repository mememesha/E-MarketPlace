namespace EM.Contracts
{
    public class Contact : EntityBase
    {
        public virtual ContactType Type { get; set; }

        public virtual string Value { get; set; }

        /// <summary>сторонние органицации могут видеть</summary>
        public virtual bool IsPublic { get; set; }
    }
}