using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ServiceEndpoints
{
    internal class GetAllServiceEndpoint
    {
    }

    public class GetAllServiceEndpointQuery : IQuery<BaseResult<IEnumerable<ServiceEndpointGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllServiceEndpointHandler : IQueryHandler<GetAllServiceEndpointQuery, BaseResult<IEnumerable<ServiceEndpointGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllServiceEndpointHandler> _logger;

        public GetAllServiceEndpointHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllServiceEndpointHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<ServiceEndpointGetDto>>> Handle(GetAllServiceEndpointQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var serviceEndpoint = _mapper.Map<IEnumerable<ServiceEndpointGetDto>>
                (await _unitOfWork.ServiceEndpointRepository.FindByQuery(null, false, se => se.Service))
                ?? throw new NotFoundException($"Service endpoint not found");

                return BaseResult<IEnumerable<ServiceEndpointGetDto>>.Success(serviceEndpoint);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllServiceEndpointHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
