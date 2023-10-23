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
public class UsersController : ControllerBase
{
    private readonly IRabbitMqService _service;
    private readonly IMapper _mapper;

    public UsersController(IRabbitMqService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}"), Authorize("write-access")]
    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        //TODO Need Saga
        var userResponse = await _service.RpcCallAsync("get_user_by_id", id.ToString());
        var setting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var user = JsonConvert.DeserializeObject<User>(userResponse, setting);

        foreach (var userWithRole in user.UsersWithRole!)
        {
            var organizationResponse = await _service.RpcCallAsync("get_organization_by_id", userWithRole.OrganizationId.ToString());
            var organization = JsonConvert.DeserializeObject<Organization>(organizationResponse, setting);

            // organization.Offers ??= new List<Offer>();
            //
            // foreach (var offerId in organization.OffersId!)
            // {
            //     var offerResponse = await _service.RpcCallAsync("get_offer_by_id", offerId.ToString());
            //     var offer = JsonConvert.DeserializeObject<Offer>(offerResponse, setting);
            //     organization.Offers!.Add(offer);
            // }
            //
            // organization.Places ??= new List<Place>();
            //
            // foreach (var placeId in organization.PlacesId!)
            // {
            //     var placeResponse = await _service.RpcCallAsync("get_place_by_id", placeId.ToString());
            //     var place = JsonConvert.DeserializeObject<Place>(placeResponse, setting);
            //     organization.Places!.Add(place);
            // }

            userWithRole.Organization = organization;
        }

        var result = _mapper.Map<UserDto>(user);

        return result;
    }
}
