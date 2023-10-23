
namespace EM.Dto
{
    public class OrganizationShortDto : ICloneable
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? OGRN { get; set; }
        public string? INN { get; set; }

        public object Clone()
        {
            return new OrganizationShortDto
            {
                Id = this.Id,
                Name = this.Name,
                OGRN = this.OGRN,
                INN = this.INN
            };
        }
    }
}