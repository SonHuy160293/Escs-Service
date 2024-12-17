using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.UserEmailServiceConfigs
{
    public class GetEmailServiceConfigDetailByEmailQuery : IQuery<BaseResult<UserEmailServiceConfigSensitiveGetDto>>
    {
        public string Email { get; set; }
    }


    // Implement the handler using the correct response type
    public class GetEmailServiceConfigDetailByEmailHandler : IQueryHandler<GetEmailServiceConfigDetailByEmailQuery, BaseResult<UserEmailServiceConfigSensitiveGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEmailServiceConfigDetailByEmailHandler> _logger;

        public GetEmailServiceConfigDetailByEmailHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetEmailServiceConfigDetailByEmailHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<UserEmailServiceConfigSensitiveGetDto>> Handle(GetEmailServiceConfigDetailByEmailQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var emailServiceConfig = _mapper.Map<UserEmailServiceConfigSensitiveGetDto>
               (await _unitOfWork.UserEmailServiceConfigurationRepository.FindEntityByQuery(
                   es => es.SmtpEmail == request.Email, false, es => es.User, es => es.Service))
               ?? throw new NotFoundException($"User email config with userId:{request.Email} not found");

                return BaseResult<UserEmailServiceConfigSensitiveGetDto>.Success(emailServiceConfig);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetEmailServiceConfigDetailByEmailHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
