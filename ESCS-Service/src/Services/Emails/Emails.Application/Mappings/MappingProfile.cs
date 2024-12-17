using AutoMapper;
using Emails.Application.Features.Emails;
using Emails.Domain.Dtos;
using Emails.Domain.Models;

namespace Emails.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EmailDto, Email>().ReverseMap();
            CreateMap<SendEmailCommand, Email>().ReverseMap();
        }
    }
}
