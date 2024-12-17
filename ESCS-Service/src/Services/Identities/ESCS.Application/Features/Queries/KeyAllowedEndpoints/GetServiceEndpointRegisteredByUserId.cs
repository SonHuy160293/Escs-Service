using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.KeyAllowedEndpoints
{
    internal class GetServiceEndpointRegisteredByUserId
    {
    }

    public class GetServiceEndpointRegisteredByUserIdQuery : IQuery<BaseResult<IEnumerable<ServiceEndpointRegisterByUserGetDto>>>
    {
        public long UserId { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetServiceEndpointRegisteredByUserIdHandler : IQueryHandler<GetServiceEndpointRegisteredByUserIdQuery, BaseResult<IEnumerable<ServiceEndpointRegisterByUserGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetServiceEndpointRegisteredByUserIdHandler> _logger;

        public GetServiceEndpointRegisteredByUserIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetServiceEndpointRegisteredByUserIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<ServiceEndpointRegisterByUserGetDto>>> Handle(GetServiceEndpointRegisteredByUserIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var keyAllowedEndpoints = (await _unitOfWork.KeyAllowedEndpointRepository
                                  .FindByQuery(k => k.UserApiKey.UserId == request.UserId, false, k => k.ServiceEndpoint, k => k.UserApiKey))

                ?? throw new NotFoundException($"service endpoint with userId:{request.UserId} not found");

                var result = keyAllowedEndpoints.Select(k => new ServiceEndpointRegisterByUserGetDto
                {
                    UserId = k.UserApiKey.UserId,
                    ServiceId = k.EndpointId,
                    Method = k.ServiceEndpoint.Method,
                    Url = k.ServiceEndpoint.Url,
                }).DistinctBy(se => se.ServiceId);

                return BaseResult<IEnumerable<ServiceEndpointRegisterByUserGetDto>>.Success(result);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetServiceEndpointRegisteredByUserIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
