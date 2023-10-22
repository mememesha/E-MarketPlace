using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using EM.WebApi.Core.Models;
using EM.WebApi.Core.Options;
using EM.WebApi.Core.Settings.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nest;

namespace EM.WebApi.Core.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchController
    {
        private readonly IOptions<ElasticsearchOptions> _options;

        public SearchController(IOptions<ElasticsearchOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// Метод апи поиска
        /// </summary>
        /// <param name="query">Строка запроса</param>
        /// <param name="location">Локация в которой ищем. По дефолту во всех локациях. Но как идея, дефолтное значение из ip из httpcontext</param>
        /// <param name="category">Список категорий в которых ищем, например кантакты, организация, продажи, покупки. По дефолту во всех категориях</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<SearchDocument>> GetSearchResult(
            [FromQuery(Name = "query")] string query,
            [FromQuery(Name = "location")] string? location,
            [FromQuery(Name = "category")] string[]? categories)
        {
            var searchResult = new List<SearchDocument>();

            var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

            var settings = new ConnectionSettings(pool)
                .DefaultMappingFor<SearchDocument>(
                    m =>
                        m.IndexName(ElasticSearchSettings.DefaultSearchIndexName));
            ;

            var client = new ElasticClient(settings);

            var elasticQuery = new MatchQuery {Field = "isDeleted", Query = "false"}
                               && (new MatchQuery {Field = "title", Query = query}
                                   || new MatchQuery {Field = "description", Query = query}
                                   || new MatchQuery {Field = "url", Query = query});
            if (location != null)
            {
                elasticQuery = elasticQuery && new MatchQuery {Field = "location", Query = location};
            }

            if (categories.Any())
            {
                var catQueries = categories
                    .Select(category => new MatchQuery {Field = "category", Query = category});
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

        /// <summary>
        /// Метод добавления документа в индекс
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="description"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpGet("Add")]
        public async Task<string> AddSearchDocument(
            [FromQuery(Name = "title")] string title,
            [FromQuery(Name = "url")] string url,
            [FromQuery(Name = "description")] string description,
            [FromQuery(Name = "category")] string category,
            [FromQuery(Name = "location")] string location)
        {
            var SearchDocument = new SearchDocument
            {
                Title = title,
                Url = url,
                Description = description,
                Category = category,
                Location = location,
                IsDeleted = false,
                DocumentId = Guid.NewGuid()
            };

            var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

            var settings = new ConnectionSettings(pool)
                .DefaultMappingFor<SearchDocument>(
                m =>
                    m.IndexName(ElasticSearchSettings.DefaultSearchIndexName));;

            var client = new ElasticClient(settings);

            var response = await client.IndexAsync(SearchDocument, 
                idx => idx.Index(ElasticSearchSettings.DefaultSearchIndexName));

            if (!response.IsValid)
            {
                throw response.OriginalException;
            }
            
            return SearchDocument.DocumentId.ToString();
        }
    }
}