using Elasticsearch.Net;
using EM.MicroService.SearchApi.Models;
using EM.MicroService.SearchApi.Options;
using Microsoft.Extensions.Options;
using Nest;

namespace EM.MicroService.SearchApi.Repository;

public class ElasticRepository : IElasticRepository
{
    private readonly IOptions<ElasticSearchOptions> _options;

    public ElasticRepository(IOptions<ElasticSearchOptions> options)
    {
        _options = options;
    }
    
    public async Task<string> AddDocument(SearchDocument document)
    {
        var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

        var settings = new ConnectionSettings(pool)
            .DefaultMappingFor<SearchDocument>(
                m =>
                    m.IndexName(GetElasticIndexName()));

        var client = new ElasticClient(settings); //TODO: Вынести клиента и замокать его в тестах?

        var response = await client.IndexAsync(document,
            idx => idx.Index(GetElasticIndexName()));

        if (!response.IsValid)
        {
            throw response.OriginalException;
        }

        return response.Id;
    }

    public async Task<IEnumerable<SearchDocument>> SearchDocuments(
        string query,
        string? location,
        string[]? categories,
        int from = 0,
        int size = 10)
    {
        var searchResult = new List<SearchDocument>();

        var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

        var settings = new ConnectionSettings(pool)
            .DefaultMappingFor<SearchDocument>(
                m =>
                    m.IndexName(GetElasticIndexName()));

        var client = new ElasticClient(settings);

        QueryBase elasticQuery = new MatchQuery { Field = "document", Query = query };
        if (location != null)
        {
            elasticQuery = elasticQuery && new MatchQuery { Field = "location", Query = location };
        }

        if (categories.Any())
        {
            var catQueries = categories
                .Select(category => new MatchQuery { Field = "category", Query = category });
            QueryBase catElasticQuery = null;
            catElasticQuery = catQueries.Aggregate(catElasticQuery, (current, catQuery) => current || catQuery);
            elasticQuery = elasticQuery && catElasticQuery;
        }

        var request = new SearchRequest
        {
            From = from,
            Size = size,
            Query = elasticQuery,
            RequestCache = false
        };

        var responseSearch = client.Search<SearchDocument>(request);

        if (!responseSearch.IsValid)
        {
            throw responseSearch.OriginalException;
        }

        searchResult.AddRange(responseSearch.Documents);

        return searchResult;
    }

    public async Task DeleteDocuments(string documentId)
    {
        var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

        var settings = new ConnectionSettings(pool)
            .DefaultMappingFor<SearchDocument>(
                m =>
                    m.IndexName(GetElasticIndexName()));

        var client = new ElasticClient(settings);
        var deleteResponse = await client.DeleteAsync<SearchDocument>(documentId);
        if (!deleteResponse.IsValid)
        {
            throw new Exception($"[{documentId}] has not deleted", deleteResponse.OriginalException);
        }
    }

    private string GetElasticIndexName()
    {
        return string.Format(_options.Value.SearchIndexPattern, DateTime.Now.ToString("yyyy-M"));
    }
}