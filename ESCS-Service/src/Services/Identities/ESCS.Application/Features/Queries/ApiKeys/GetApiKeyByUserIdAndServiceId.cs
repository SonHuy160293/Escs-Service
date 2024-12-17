using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ApiKeys
{
    class GetApiKeyByUserIdAndServiceIdAndServiceId
    {
    }

    public class GetApiKeyByUserIdAndServiceIdQuery : IQuery<BaseResult<IEnumerable<UserApiKeyGetDto>>>
    {
        public long UserId { get; set; }
        public long ServiceId { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetApiKeyByUserIdAndServiceIdHandler : IQueryHandler<GetApiKeyByUserIdAndServiceIdQuery, BaseResult<IEnumerable<UserApiKeyGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApiKeyByUserIdAndServiceIdHandler> _logger;

        public GetApiKeyByUserIdAndServiceIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetApiKeyByUserIdAndServiceIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<UserApiKeyGetDto>>> Handle(GetApiKeyByUserIdAndServiceIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var userApiKeys = _mapper.Map<IEnumerable<UserApiKeyGetDto>>
                (await _unitOfWork.UserApiKeyRepository.FindByQuery(u => u.UserId == request.UserId && u.ServiceId == request.ServiceId, false, u => u.Service, u => u.User))
                ?? throw new NotFoundException($"Api key of user with id{request.UserId} not found");


                return BaseResult<IEnumerable<UserApiKeyGetDto>>.Success(userApiKeys);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllUserApiKeysHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
