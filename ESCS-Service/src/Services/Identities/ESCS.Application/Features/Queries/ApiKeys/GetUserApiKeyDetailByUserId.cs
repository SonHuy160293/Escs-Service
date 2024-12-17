using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ApiKeys
{
    internal class GetUserApiKeyDetailByUserId
    {
    }

    public class GetUserApiKeyDetailByUserIdQuery : IQuery<BaseResult<IEnumerable<UserApiKeyDetailDto>>>
    {
        public long UserId { get; set; }
        public long ServiceId { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetUserApiKeyDetailByUserIdHandler : IQueryHandler<GetUserApiKeyDetailByUserIdQuery, BaseResult<IEnumerable<UserApiKeyDetailDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserApiKeyDetailByUserIdHandler> _logger;

        public GetUserApiKeyDetailByUserIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetUserApiKeyDetailByUserIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<UserApiKeyDetailDto>>> Handle(GetUserApiKeyDetailByUserIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var userApiKeys = _mapper.Map<IEnumerable<UserApiKeyDetailDto>>
                (await _unitOfWork.UserApiKeyRepository.FindByQuery(u => u.UserId == request.UserId && u.ServiceId == request.ServiceId, false, u => u.Service, u => u.User))
                ?? throw new NotFoundException($"Api key of user with id{request.UserId} not found");


                foreach (var userApiKey in userApiKeys)
                {
                    var allowedEnpointsDetail = _mapper.Map<List<KeyAllowedEndpointDetailDto>>
                                            (await _unitOfWork.KeyAllowedEndpointRepository.FindByQuery(k => k.UserApiKeyId == userApiKey.Id, false, k => k.ServiceEndpoint));

                    userApiKey.AllowedEndpoints = allowedEnpointsDetail;
                }


                return BaseResult<IEnumerable<UserApiKeyDetailDto>>.Success(userApiKeys);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetUserApiKeyDetailByUserIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
