namespace EM.Contracts;

public class Category : EntityBase
{
    public virtual string? Name { get; set; }
    public virtual List<Category>? SubCategories { get; set; }
    public virtual List<OfferDescription>? OfferDescriptions { get; set; }
}