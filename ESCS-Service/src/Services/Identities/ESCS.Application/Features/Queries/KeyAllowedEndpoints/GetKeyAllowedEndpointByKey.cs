using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.KeyAllowedEndpoints
{
    internal class GetKeyAllowedEndpointByKey
    {
    }

    public class GetKeyAllowedEndpointByKeyIdQuery : IQuery<BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>>
    {
        public long KeyId { get; set; }
    }


    // Implement the handler using the correct response type
    public class GetKeyAllowedEndpointByKeyIdHandler : IQueryHandler<GetKeyAllowedEndpointByKeyIdQuery, BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetKeyAllowedEndpointByKeyIdHandler> _logger;

        public GetKeyAllowedEndpointByKeyIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetKeyAllowedEndpointByKeyIdHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>> Handle(GetKeyAllowedEndpointByKeyIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var KeyAllowedEndpoints = _mapper.Map<IEnumerable<KeyAllowedEndpointGetDto>>
                (await _unitOfWork.KeyAllowedEndpointRepository.FindByQuery(k => k.UserApiKeyId == request.KeyId, false, u => u.ServiceEndpoint, u => u.UserApiKey))
                 ?? throw new NotFoundException($"Key allowed endpoint with keyId:{request.KeyId} not found");

                return BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>.Success(KeyAllowedEndpoints);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetKeyAllowedEndpointByKeyIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
