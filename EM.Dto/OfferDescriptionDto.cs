using System;

namespace EM.Dto
{
    public class OfferDescriptionDto : ICloneable
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public CategoryDto? CategoryDto { get; set; }
        public bool IsSale { get; set; }
        public string? Image { get; set; }

        public object Clone()
        {
            return new OfferDescriptionDto
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                CategoryDto = (CategoryDto)CategoryDto!.Clone(),
                IsSale = this.IsSale,
                Image = this.Image
            };
        }
    }
}