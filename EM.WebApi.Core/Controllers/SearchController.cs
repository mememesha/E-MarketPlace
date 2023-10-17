using System.Collections.Generic;
using System.Threading.Tasks;
using EM.WebApi.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EM.WebApi.Core.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchController
    {
        // Фильтр по: Местности (города) (Дефолтное Москва), Кантактам, Организация, Продажи, Покупки
        [HttpGet]
        public async Task<IEnumerable<SearchResultResponse>> GetSearchResult(
            [FromQuery(Name = "query")] string query, 
            [FromQuery(Name = "filters")] string[] filters)
        {
            var searchResult = new List<SearchResultResponse>();
            searchResult.Add(new SearchResultResponse
            {
                Title = "Search offer result Title",
                Url = "https://offer.ru/",
                Description = "Search offer result description"
            });
            
            return searchResult;
        }
    }
}