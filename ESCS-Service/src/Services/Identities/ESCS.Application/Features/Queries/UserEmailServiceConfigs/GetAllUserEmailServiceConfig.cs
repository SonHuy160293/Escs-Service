using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.UserEmailServiceConfigs
{
    internal class GetAllUserEmailServiceConfig
    {
    }

    public class GetAllEmailServiceConfigsQuery : IQuery<BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllEmailServiceConfigsHandler : IQueryHandler<GetAllEmailServiceConfigsQuery, BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllEmailServiceConfigsHandler> _logger;

        public GetAllEmailServiceConfigsHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllEmailServiceConfigsHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>> Handle(GetAllEmailServiceConfigsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var emailServiceConfigs = _mapper.Map<IEnumerable<UserEmailServiceConfigGetDto>>
                (await _unitOfWork.UserEmailServiceConfigurationRepository.FindByQuery(null, false, es => es.User, es => es.Service))
                ?? throw new NotFoundException($"User email config not found");

                return BaseResult<IEnumerable<UserEmailServiceConfigGetDto>>.Success(emailServiceConfigs);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllEmailServiceConfigsHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
