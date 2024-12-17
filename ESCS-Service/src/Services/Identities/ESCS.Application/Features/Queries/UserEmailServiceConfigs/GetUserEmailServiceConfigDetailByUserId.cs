using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.UserEmailServiceConfigs
{
    internal class GetUserEmailServiceConfigDetailByUserId
    {
    }


    public class GetEmailServiceConfigDetailByUserIdQuery : IQuery<BaseResult<IEnumerable<UserEmailServiceConfigSensitiveGetDto>>>
    {
        public long UserId { get; set; }
    }


    // Implement the handler using the correct response type
    public class GetEmailServiceConfigDetailByUserIdHandler : IQueryHandler<GetEmailServiceConfigDetailByUserIdQuery, BaseResult<IEnumerable<UserEmailServiceConfigSensitiveGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEmailServiceConfigDetailByUserIdHandler> _logger;

        public GetEmailServiceConfigDetailByUserIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetEmailServiceConfigDetailByUserIdHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<UserEmailServiceConfigSensitiveGetDto>>> Handle(GetEmailServiceConfigDetailByUserIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var emailServiceConfigs = _mapper.Map<IEnumerable<UserEmailServiceConfigSensitiveGetDto>>
                                            (await _unitOfWork.UserEmailServiceConfigurationRepository.FindByQuery(
                                                    es => es.UserId == request.UserId, false, es => es.User, es => es.Service))
                ?? throw new NotFoundException($"User email config with userId:{request.UserId} not found");

                return BaseResult<IEnumerable<UserEmailServiceConfigSensitiveGetDto>>.Success(emailServiceConfigs);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetEmailServiceConfigDetailByUserIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
