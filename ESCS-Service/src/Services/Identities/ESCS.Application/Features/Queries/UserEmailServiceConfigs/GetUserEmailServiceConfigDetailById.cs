using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.UserEmailServiceConfigs
{
    internal class GetUserEmailServiceConfigDetailById
    {
    }
    public class GetEmailServiceConfigDetailByIdQuery : IQuery<BaseResult<UserEmailServiceConfigSensitiveGetDto>>
    {
        public long Id { get; set; }
    }


    // Implement the handler using the correct response type
    public class GetEmailServiceConfigDetailByIdHandler : IQueryHandler<GetEmailServiceConfigDetailByIdQuery, BaseResult<UserEmailServiceConfigSensitiveGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEmailServiceConfigDetailByIdHandler> _logger;

        public GetEmailServiceConfigDetailByIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetEmailServiceConfigDetailByIdHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<UserEmailServiceConfigSensitiveGetDto>> Handle(GetEmailServiceConfigDetailByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var emailServiceConfigs = _mapper.Map<UserEmailServiceConfigSensitiveGetDto>
                                            (await _unitOfWork.UserEmailServiceConfigurationRepository.GetById(
                                                     request.Id, es => es.User, es => es.Service))
                ?? throw new NotFoundException($"User email config with userId:{request.Id} not found");

                return BaseResult<UserEmailServiceConfigSensitiveGetDto>.Success(emailServiceConfigs);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetEmailServiceConfigDetailByIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
