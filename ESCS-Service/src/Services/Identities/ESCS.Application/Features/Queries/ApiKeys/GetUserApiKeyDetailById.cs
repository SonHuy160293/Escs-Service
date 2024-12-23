using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ApiKeys
{
    class GetUserApiKeyDetailById
    {
    }

    public class GetUserApiKeyDetailByIdQuery : IQuery<BaseResult<UserApiKeyDetailDto>>
    {
        public long Id { get; set; }

    }

    // Implement the handler using the correct response type
    public class GetUserApiKeyDetailByIdHandler : IQueryHandler<GetUserApiKeyDetailByIdQuery, BaseResult<UserApiKeyDetailDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserApiKeyDetailByIdHandler> _logger;

        public GetUserApiKeyDetailByIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetUserApiKeyDetailByIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<UserApiKeyDetailDto>> Handle(GetUserApiKeyDetailByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var userApiKey = _mapper.Map<UserApiKeyDetailDto>
                (await _unitOfWork.UserApiKeyRepository.FindEntityByQuery(u => u.Id == request.Id, false, u => u.Service, u => u.User))
                ?? throw new NotFoundException($"Api key of user with id{request.Id} not found");


                var allowedEnpointsDetail = _mapper.Map<List<KeyAllowedEndpointDetailDto>>
                                            (await _unitOfWork.KeyAllowedEndpointRepository.FindByQuery(k => k.UserApiKeyId == userApiKey.Id, false, k => k.ServiceEndpoint));

                userApiKey.AllowedEndpoints = allowedEnpointsDetail;


                return BaseResult<UserApiKeyDetailDto>.Success(userApiKey);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetUserApiKeyDetailByIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
