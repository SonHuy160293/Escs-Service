using AutoMapper;
using ESCS.Application.Features.Commands.Roles;
using ESCS.Application.Features.Commands.ServiceEndpoints;
using ESCS.Application.Features.Commands.Services;
using ESCS.Application.Features.Commands.UserEmailServiceConfigs;
using ESCS.Application.Features.Commands.Users;
using ESCS.Domain.Dtos;
using ESCS.Domain.Models;

namespace ESCS.Application.Mappings
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {

            //user mapping
            CreateMap<CreateUserCommand, User>().ReverseMap();
            CreateMap<UserGetDto, User>().ReverseMap();


            //service mapping
            CreateMap<CreateServiceCommand, Service>().ReverseMap();
            CreateMap<ServiceGetDto, Service>().ReverseMap();
            CreateMap<UserEmailServiceConfiguration, UserInServiceGetDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));


            //emailConfig mapping
            CreateMap<CreateEmailServiceConfigCommand, UserEmailServiceConfiguration>().ReverseMap();
            CreateMap<UserEmailServiceConfigGetDto, UserEmailServiceConfiguration>().ReverseMap();
            CreateMap<UserEmailServiceConfigSensitiveGetDto, UserEmailServiceConfiguration>().ReverseMap();

            //role mapping
            CreateMap<CreateRoleCommand, Role>().ReverseMap();
            CreateMap<RoleGetDto, Role>().ReverseMap();

            //apiKey mapping
            CreateMap<UserApiKey, UserApiKeyGetDto>().ReverseMap();
            CreateMap<UserApiKey, UserApiKeyDetailDto>().ReverseMap();

            //endpoint mapping
            CreateMap<ServiceEndpoint, ServiceEndpointGetDto>().ReverseMap();
            CreateMap<CreateServiceEndpointCommand, ServiceEndpoint>().ReverseMap();


            //keyAllowedEndpoint mapping
            CreateMap<KeyAllowedEndpoint, KeyAllowedEndpointGetDto>().ReverseMap();
            CreateMap<KeyAllowedEndpoint, KeyAllowedEndpointDetailDto>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.ServiceEndpoint.Method))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.ServiceEndpoint.IsActive))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.ServiceEndpoint.Url))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ServiceEndpoint.Description));

        }
    }
}
