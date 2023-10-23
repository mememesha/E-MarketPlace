using EM.MicroService.SearchApi.Models;

namespace EM.MicroService.SearchApi.Repository;

public interface IElasticRepository
{
    Task<string> AddDocument(SearchDocument document);
    
    Task<IEnumerable<SearchDocument>> SearchDocuments(
        string query,
        string? location,
        string[]? categories,
        int from = 0, 
        int size = 10);
    
    Task DeleteDocuments(string documentId);
}