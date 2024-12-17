using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ApiKeys
{
    internal class GetApiKeyByUserId
    {
    }

    public class GetApiKeyByUserIdQuery : IQuery<BaseResult<IEnumerable<UserApiKeyGetDto>>>
    {
        public long UserId { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetApiKeyByUserIdHandler : IQueryHandler<GetApiKeyByUserIdQuery, BaseResult<IEnumerable<UserApiKeyGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApiKeyByUserIdHandler> _logger;

        public GetApiKeyByUserIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetApiKeyByUserIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<UserApiKeyGetDto>>> Handle(GetApiKeyByUserIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var userApiKeys = _mapper.Map<IEnumerable<UserApiKeyGetDto>>
                (await _unitOfWork.UserApiKeyRepository.FindByQuery(u => u.UserId == request.UserId, false, u => u.Service, u => u.User))
                ?? throw new NotFoundException($"Api key of user with id{request.UserId} not found");


                return BaseResult<IEnumerable<UserApiKeyGetDto>>.Success(userApiKeys);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetApiKeyByUserIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
