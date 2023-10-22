using System.ComponentModel.DataAnnotations;

namespace EM.MicroService.SearchApi.Options;

public class ElasticSearchOptions
{
    [Required(ErrorMessage = "SearchIndexPattern is required parameter in appsettings")]
    public string SearchIndexPattern { get; set; }
    
    [Required(ErrorMessage = "Uri is required parameter in appsettings")]
    public string Uri { get; set; }
}