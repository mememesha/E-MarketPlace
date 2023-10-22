using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace EM.WebApi.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly IRabbitMqService _service;

    public LocationsController(IRabbitMqService service)
    {
        _service = service;
    }

    [HttpGet("current")]
    public async Task<string?> GetCurrent()
    {
        return await _service.RpcCallAsync("get_location", HttpContext.Connection.RemoteIpAddress!.ToString());
    }
}
