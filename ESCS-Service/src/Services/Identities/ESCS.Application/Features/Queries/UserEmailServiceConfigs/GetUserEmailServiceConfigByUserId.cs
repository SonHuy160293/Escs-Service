using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.UserEmailServiceConfigs
{
    internal class GetUserEmailServiceConfigByUserId
    {
    }


    public class GetEmailServiceConfigByUserIdQuery : IQuery<BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>>
    {
        public long UserId { get; set; }
    }


    // Implement the handler using the correct response type
    public class GetEmailServiceConfigByUserIdHandler : IQueryHandler<GetEmailServiceConfigByUserIdQuery, BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEmailServiceConfigByUserIdHandler> _logger;

        public GetEmailServiceConfigByUserIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetEmailServiceConfigByUserIdHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>> Handle(GetEmailServiceConfigByUserIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var emailServiceConfigs = _mapper.Map<IEnumerable<UserEmailServiceConfigGetDto>>
                (await _unitOfWork.UserEmailServiceConfigurationRepository.FindByQuery(
                    es => es.UserId == request.UserId, false, es => es.User, es => es.Service))
                ?? throw new NotFoundException($"User email config with userId:{request.UserId} not found");

                return BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>.Success(emailServiceConfigs);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetEmailServiceConfigByUserIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
