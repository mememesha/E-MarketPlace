

namespace EM.Dto
{
    public class PlaceDto : ICloneable
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }

        public object Clone()
        {
            return new PlaceDto
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                Region = this.Region,
                City = this.City,
                Address = this.Address
            };
        }
    }
}