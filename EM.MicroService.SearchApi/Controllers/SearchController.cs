using Elasticsearch.Net;
using EM.MicroService.SearchApi.Abstractions;
using EM.MicroService.SearchApi.Helpers;
using EM.MicroService.SearchApi.Models;
using EM.MicroService.SearchApi.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;

namespace EM.MicroService.SearchApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchController : Controller
    {
        private readonly IOptions<ElasticSearchOptions> _options;
        private readonly IDistributedCache _distributedCache;

        public SearchController(IOptions<ElasticSearchOptions> options,
            IDistributedCache distributedCache)
        {
            _options = options;
            _distributedCache = distributedCache;
        }
        
        /// <summary>
        /// Метод апи поиска
        /// </summary>
        /// <param name="query">Строка запроса</param>
        /// <param name="location">Локация в которой ищем. По дефолту во всех локациях. Но как идея, дефолтное значение из ip из httpcontext</param>
        /// <param name="category">Список категорий в которых ищем, например кантакты, организация, продажи, покупки. По дефолту во всех категориях</param>
        /// <returns></returns>
        // [HttpGet, Authorize("write-access")]
        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<SearchDocument>> GetSearchResult(
            [FromQuery(Name = "query")] string query,
            [FromQuery(Name = "location")] string? location,
            [FromQuery(Name = "category")] string[]? categories)
        {
            var redisKey = RedisKeyHelper.GetRedisKey(JsonConvert.SerializeObject(HttpContext.Request.Query));

            var searchResult = new List<SearchDocument>();

            var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

            var settings = new ConnectionSettings(pool)
                .DefaultMappingFor<SearchDocument>(
                    m =>
                        m.IndexName(GetElasticIndexName()));

            var client = new ElasticClient(settings);

            var elasticQuery = new MatchQuery {Field = "isDeleted", Query = "false"}
                               && (new MatchQuery {Field = "document", Query = query});
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
        /// <param name="document"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<string> AddSearchDocument(
            [FromBody] string document,
            [FromQuery(Name = "category")] string category,
            [FromQuery(Name = "location")] string location)
        {
            var SearchDocument = new SearchDocument
            {
                Document = document,
                Category = category,
                Location = location,
                IsDeleted = false,
                DocumentId = Guid.NewGuid()
            };

            var pool = new SingleNodeConnectionPool(new Uri(_options.Value.Uri!));

            var settings = new ConnectionSettings(pool)
                .DefaultMappingFor<SearchDocument>(
                m =>
                    m.IndexName(GetElasticIndexName()));

            var client = new ElasticClient(settings);

            var response = await client.IndexAsync(SearchDocument, 
                idx => idx.Index(GetElasticIndexName()));

            if (!response.IsValid)
            {
                throw response.OriginalException;
            }
            
            return SearchDocument.DocumentId.ToString();
        }
        
        private string GetElasticIndexName()
        {
            return string.Format(_options.Value.SearchIndexPattern, DateTime.Now.ToString("yyyy-M"));
        }
    }
}