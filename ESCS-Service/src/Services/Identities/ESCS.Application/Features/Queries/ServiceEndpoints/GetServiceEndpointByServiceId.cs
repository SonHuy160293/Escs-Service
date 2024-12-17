using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ServiceEndpoints
{
    internal class GetServiceEndpointByServiceId
    {
    }

    public class GetServiceEndpointByServiceIdQuery : IQuery<BaseResult<IEnumerable<ServiceEndpointGetDto>>>
    {
        public long ServiceId { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetServiceEndpointByServiceIdHandler : IQueryHandler<GetServiceEndpointByServiceIdQuery, BaseResult<IEnumerable<ServiceEndpointGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetServiceEndpointByServiceIdHandler> _logger;

        public GetServiceEndpointByServiceIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetServiceEndpointByServiceIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<ServiceEndpointGetDto>>> Handle(GetServiceEndpointByServiceIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var serviceEndpoint = _mapper.Map<IEnumerable<ServiceEndpointGetDto>>
                (await _unitOfWork.ServiceEndpointRepository.FindByQuery(se => se.ServiceId == request.ServiceId, false, se => se.Service))
                ?? throw new NotFoundException($"service endpoint with serviceId:{request.ServiceId} not found");

                return BaseResult<IEnumerable<ServiceEndpointGetDto>>.Success(serviceEndpoint);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetServiceEndpointByServiceIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
