using Elasticsearch.Net;
using EM.MicroService.SearchApi.Models;
using EM.MicroService.SearchApi.Options;
using Microsoft.Extensions.Options;
using Nest;
using EM.Dto;

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

        var client = new ElasticClient(settings);

        var response = await client.IndexAsync(document,
            idx => idx.Index(GetElasticIndexName()));

        if (!response.IsValid)
        {
            throw response.OriginalException;
        }

        return response.Id;
    }

    public async Task<IEnumerable<OfferShortResponseDto>> SearchAsync(string query, string? category)
    {
        await Task.Delay(10);

        var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

        var settings = new ConnectionSettings(pool)
           .DefaultMappingFor<OfferShortResponseDto>(
               m =>
                   m.IndexName(_options.Value.SearchIndexPattern));

        var client = new ElasticClient(settings);

        var searchResponse = client.Search<OfferShortResponseDto>(s => s
            .Query(q => q
                .Bool(b => b
                    .Must(mu => mu
                        .Match(m => m
                            .Field(f => f.Category)
                            .Query(category)
                        ), mu => mu
                        .Match(m => m
                            .Field(f => f.Title)
                            .Query(query)
                        )
                    )
                )
            )
        );
        var resultA = searchResponse.Documents;
        return resultA;
    }

    public async Task<IEnumerable<SearchDocument>> SearchDocuments(string query, string? location, string[]? categories)
    {
        var searchResult = new List<SearchDocument>();

        var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

        var settings = new ConnectionSettings(pool)
            .DefaultMappingFor<SearchDocument>(
                m =>
                    m.IndexName(GetElasticIndexName()));

        var client = new ElasticClient(settings);

        var elasticQuery = new MatchQuery { Field = "isDeleted", Query = "false" }
                           && (new MatchQuery { Field = "document", Query = query });
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
            From = 0,
            Size = 10,
            Query = elasticQuery
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