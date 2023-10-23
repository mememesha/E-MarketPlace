using AutoMapper;
using EM.Contracts;
using EM.Dto;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EM.WebApi.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReservesController : ControllerBase
{
    private readonly IRabbitMqService _service;
    private readonly IMapper _mapper;

    public ReservesController(IRabbitMqService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    ///     Добавить новый резерв
    /// </summary>
    /// <param name="reserveRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize("write-access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAsync(ReserveRequestDto? reserveRequest)
    {
        //TODO Need to make Saga
        if (reserveRequest is null)
            return BadRequest(new ArgumentNullException(nameof(reserveRequest)));

        var reserve = _mapper.Map<Reserve>(reserveRequest);
        reserve.Id = Guid.NewGuid();

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var reserveAddedResponse =
            await _service.RpcCallAsync("add_reserve", JsonConvert.SerializeObject(reserve, setting));

        var reserveAddedToOfferResponse =
            await _service.RpcCallAsync("add_reserve_to_offer", JsonConvert.SerializeObject(reserve, setting));

        if (JsonConvert.DeserializeObject<bool>(reserveAddedResponse) &&
            JsonConvert.DeserializeObject<bool>(reserveAddedToOfferResponse))
            return Ok();
        else
            return StatusCode(500);
    }

    /// <summary>
    /// Получить все резервы по оферу
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("offers/{id:guid}")]
    [Authorize("write-access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByOfferId(Guid? id)
    {
        if (id is null)
            return BadRequest(new ArgumentNullException(nameof(id)));

        var reservesResponse = await _service.RpcCallAsync("get_reserves_by_offer_id", id.ToString());

        if (string.IsNullOrEmpty(reservesResponse))
            return NotFound(null);

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var reserve = JsonConvert.DeserializeObject<Reserve>(reservesResponse, setting);
        var result = _mapper.Map<ReserveDto>(reserve);

        return Ok(result);
    }
}