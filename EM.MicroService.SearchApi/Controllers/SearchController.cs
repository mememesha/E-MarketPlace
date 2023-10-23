using EM.MicroService.SearchApi.Models;
using EM.MicroService.SearchApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EM.MicroService.SearchApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchController : Controller
    {
        private readonly IElasticRepository _elasticRepository;

        public SearchController(IElasticRepository elasticRepository)
        {
            _elasticRepository = elasticRepository;
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
            return await _elasticRepository.SearchDocuments(query, location, categories);
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
            [FromQuery(Name = "category")] string? category,
            [FromQuery(Name = "location")] string? location)
        {
            var SearchDocument = new SearchDocument
            {
                Document = document,
                Category = category,
                Location = location,
            };

            return await _elasticRepository.AddDocument(SearchDocument);
        }

        // delete document
        /// <summary>
        /// Удаление документа по _id из эластика
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        [HttpDelete("Delete")]
        public async Task<string> DeleteSearchDocument(
            [FromQuery(Name = "documentId")] string documentId)
        {
            try
            {
                await _elasticRepository.DeleteDocuments(documentId);
                return $"[{documentId}] has been deleted";
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Can not delete [{documentId}] document", e);
            }
        }
    }
}