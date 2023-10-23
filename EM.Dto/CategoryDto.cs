namespace EM.Dto;

public class CategoryDto : ICloneable
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public object Clone()
    {
        return new CategoryDto
        {
            Id = this.Id,
            Name = this.Name
        };
    }
}
