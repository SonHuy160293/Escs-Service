using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.ServiceEndpoints
{
    internal class GetServiceEndpointById
    {
    }

    public class GetServiceEndpointByIdQuery : IQuery<BaseResult<ServiceEndpointGetDto>>
    {
        public long Id { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetServiceEndpointByIdHandler : IQueryHandler<GetServiceEndpointByIdQuery, BaseResult<ServiceEndpointGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetServiceEndpointByIdHandler> _logger;

        public GetServiceEndpointByIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetServiceEndpointByIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<ServiceEndpointGetDto>> Handle(GetServiceEndpointByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var serviceEndpoint = _mapper.Map<ServiceEndpointGetDto>
                (await _unitOfWork.ServiceEndpointRepository.FindEntityByQuery(se => se.Id == request.Id, false, se => se.Service))
                ?? throw new NotFoundException($"Service endpoint with Id:{request.Id} not found");

                return BaseResult<ServiceEndpointGetDto>.Success(serviceEndpoint);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetServiceEndpointByIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
