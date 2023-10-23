using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace EM.WebApi.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PlacesController : ControllerBase
{
    private readonly IRabbitMqService _service;

    public PlacesController(IRabbitMqService service)
    {
        _service = service;
    }

    [HttpGet("current")]
    public async Task<string?> GetCurrent()
    {
        return await _service.RpcCallAsync("get_current_city", HttpContext.Connection.RemoteIpAddress!.ToString());
    }

    [HttpGet("all_cities")]
    public async Task<string?> GetAllCities()
    {
        return await _service.RpcCallAsync("get_all_cities");
    }
}
