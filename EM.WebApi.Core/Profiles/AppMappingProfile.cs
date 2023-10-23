using AutoMapper;
using EM.Contracts;
using EM.Dto;

namespace EM.WebApi.Core.Profiles;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<Offer, OfferShortResponseDto>()
            .ForMember(d => d.Title, s => s.MapFrom(o => o.OfferDescription!.Title))
            .ForMember(d => d.Image, s => s.MapFrom(o => o.OfferDescription!.Image))
            .ForMember(d => d.Category, s => s.MapFrom(o => o.OfferDescription!.Category!.Name));

        CreateMap<Category, CategoryDto>()
            .ReverseMap();

        CreateMap<Offer, OfferDto>()
            .ForMember(oDto => oDto.OfferDescriptionDto, s => s.MapFrom(o => o.OfferDescription))
            .ForMember(oDto => oDto.PlaceDto, s => s.MapFrom(o => o.Place))
            .ForMember(oDto => oDto.OrganizationShortDto, s => s.MapFrom(o => o.Organization))
            .ReverseMap();

        CreateMap<OfferDescription, OfferDescriptionDto>()
            .ForMember(odDto => odDto.CategoryDto, s => s.MapFrom(od => od.Category))
            .ReverseMap();

        CreateMap<Organization, OrganizationShortDto>()
            .ReverseMap();

        CreateMap<User, UserDto>()
            .ForMember(userDto => userDto.UserWithRoleDtos, source => source.MapFrom(user => user.UsersWithRole));

        CreateMap<UserWithRole, UserWithRoleDto>()
            .ForMember(uwrDto => uwrDto.OrganizationShortDto, source => source.MapFrom(uwr => uwr.Organization));

        CreateMap<Organization, OrganizationDto>()
            .ForMember(orgDto => orgDto.OfferShortResponseDtos, source => source.MapFrom(org => org.Offers))
            .ForMember(orgDto => orgDto.PlaceDtos, source => source.MapFrom(org => org.Places));

        CreateMap<Place, PlaceDto>()
            .ReverseMap();

        CreateMap<Reserve, ReserveRequestDto>()
            .ReverseMap();

        CreateMap<Reserve, ReserveDto>()
            .ReverseMap();
    }
}