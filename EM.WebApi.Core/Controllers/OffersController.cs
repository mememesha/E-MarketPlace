using EM.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EM.WebApi.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OffersController : ControllerBase
{
    [HttpGet("new")]
    public async Task<IEnumerable<OfferDto>> GetNew()
    {
        return await FakeOfferRepository.GetNewAsync();
    }

    [HttpGet("categories")]
    public async Task <IDictionary<string,string>> GetCategories()
    {
        return await FakeOfferRepository.GetCategoriesAsync();
    }
}

internal static class FakeOfferRepository
{
    internal static async Task<IEnumerable<OfferDto>> GetNewAsync()
    {
        return await Task.FromResult(new List<OfferDto>
        {
            new OfferDto
            {
                Id = Guid.NewGuid(),
                Title = "Заголовок1",
                Description = "Описание1",
                Tags = new List<string>(),
                IsSale = true,
                CostOfUnit = 100,
                Image = null
            },
            new OfferDto
            {
                Id = Guid.NewGuid(),
                Title = "Заголовок2",
                Description = "Описание2",
                Tags = new List<string>(),
                IsSale = false,
                CostOfUnit = 200,
                Image = null
            }
        });
    }

    internal static async Task <IDictionary<string,string>> GetCategoriesAsync()
    {
        return await Task.FromResult(new Dictionary<string,string> {
            {"Все категории","all"},
            {"Автомобили","auto"},
            {"продукты","food"},
            {"Электроника","electronics"},
        });
    }
}