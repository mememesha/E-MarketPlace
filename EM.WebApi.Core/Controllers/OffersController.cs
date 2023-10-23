using System.Security.Claims;
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
public class OffersController : ControllerBase
{
    private readonly IRabbitMqService _service;
    private readonly IMapper _mapper;

    public OffersController(IRabbitMqService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    public async Task<OfferDto> GetOfferById(Guid id)
    {
        //TODO Need Saga
        var offerResponse = await _service.RpcCallAsync("get_offer_by_id", id.ToString());

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var offer = JsonConvert.DeserializeObject<Offer>(offerResponse, setting);

        var placeResponse = await _service.RpcCallAsync("get_place_by_id", offer.PlaceId.ToString());
        offer.Place = JsonConvert.DeserializeObject<Place>(placeResponse, setting);

        var organizationResponse =
            await _service.RpcCallAsync("get_organization_by_id", offer.OrganizationId.ToString());
        offer.Organization = JsonConvert.DeserializeObject<Organization>(organizationResponse, setting);

        var result = _mapper.Map<OfferDto>(offer);

        return result;
    }

    [HttpGet("new_sales")]
    public async Task<IEnumerable<OfferShortResponseDto>> GetNewSalesOffers()
    {
        var newOffersResponse = await _service.RpcCallAsync("get_new_offers", "true");

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var newOffers = JsonConvert.DeserializeObject<List<Offer>>(newOffersResponse, setting);

        var result = new List<OfferShortResponseDto>();

        foreach (var newOffer in newOffers!) result.Add(_mapper.Map<OfferShortResponseDto>(newOffer));

        return result;
    }

    [HttpGet("new_offers")]
    public async Task<IEnumerable<OfferShortResponseDto>> GetNewOffers()
    {
        var newOffersResponse = await _service.RpcCallAsync("get_new_offers", "false");

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var newOffers = JsonConvert.DeserializeObject<List<Offer>>(newOffersResponse, setting);

        var result = new List<OfferShortResponseDto>();

        foreach (var newOffer in newOffers!) result.Add(_mapper.Map<OfferShortResponseDto>(newOffer));

        return result;
    }

    [HttpGet("categories")]
    public async Task<IEnumerable<CategoryDto>> GetCategories()
    {
        var categoriesResponse = await _service.RpcCallAsync("get_all_categories");

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesResponse, setting);

        var result = new List<CategoryDto>();

        foreach (var category in categories!) result.Add(_mapper.Map<CategoryDto>(category));

        return result;
    }

    /// <summary>
    ///     Редактировать данные офера
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Authorize("write-access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOfferAsync(Guid id, OfferDto? request)
    {
        if (request is null)
            return BadRequest(new ArgumentNullException(nameof(request)));

        var offerResponse = await _service.RpcCallAsync("get_offer_by_id", id.ToString());

        if (offerResponse is null)
            return NotFound($"Офер с id {id} не найден.");

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var offerSource = JsonConvert.DeserializeObject<Offer>(offerResponse, setting);

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Forbid("Пользователь не авторизован");
        var userResponse = await _service.RpcCallAsync("get_user_by_id", userId);
        var user = JsonConvert.DeserializeObject<User>(userResponse, setting);
        if (user.UsersWithRole!.All(uwr => uwr.OrganizationId != offerSource.OrganizationId))
            return Forbid("Пользователь не может редактировать данный офер");

        var offerRequest = _mapper.Map<Offer>(request);
        var offerDescriptionRequest = _mapper.Map<OfferDescription>(request.OfferDescriptionDto);
        var placeRequest = _mapper.Map<Place>(request.PlaceDto);

        var offerUpdateResponse =
            await _service.RpcCallAsync("update_offer", JsonConvert.SerializeObject(offerRequest, setting));

        var offerDescriptionUpdateResponse =
            await _service.RpcCallAsync("update_offerdescription", JsonConvert.SerializeObject(offerDescriptionRequest, setting));

        var placeUpdateResponse =
            await _service.RpcCallAsync("update_place", JsonConvert.SerializeObject(placeRequest, setting));

        if (JsonConvert.DeserializeObject<bool>(offerUpdateResponse, setting) &&
            JsonConvert.DeserializeObject<bool>(offerDescriptionUpdateResponse, setting) &&
            JsonConvert.DeserializeObject<bool>(placeUpdateResponse, setting))
            return Ok();
        else
            return StatusCode(500);
    }

    [HttpGet("organizations/{id:guid}")]
    public async Task<IEnumerable<OfferShortResponseDto>> GetOffersByOrganizationId(Guid id)
    {
        var offersResponse = await _service.RpcCallAsync("get_offers_by_organizations_id", id.ToString());

        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var offers = JsonConvert.DeserializeObject<List<Offer>>(offersResponse, setting);

        var result = new List<OfferShortResponseDto>();

        foreach (var offer in offers!) result.Add(_mapper.Map<OfferShortResponseDto>(offer));

        return result;
    }
}