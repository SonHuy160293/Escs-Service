using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ApiKeys
{
    internal class GetAllApiKey
    {
    }

    public class GetAllUserApiKeysQuery : IQuery<BaseResult<IEnumerable<UserApiKeyGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllUserApiKeysHandler : IQueryHandler<GetAllUserApiKeysQuery, BaseResult<IEnumerable<UserApiKeyGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUserApiKeysHandler> _logger;

        public GetAllUserApiKeysHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllUserApiKeysHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<UserApiKeyGetDto>>> Handle(GetAllUserApiKeysQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var userApiKeys = _mapper.Map<IEnumerable<UserApiKeyGetDto>>(await _unitOfWork.UserApiKeyRepository.FindByQuery(null, false, u => u.Service, u => u.User));

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
