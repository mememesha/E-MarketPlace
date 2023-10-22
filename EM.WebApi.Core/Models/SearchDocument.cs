using System;

namespace EM.WebApi.Core.Models
{
    public class SearchDocument
    {
        public Guid DocumentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public bool IsDeleted { get; set; }
    }
}