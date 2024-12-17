using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Services
{
    internal class GetAllServices
    {
    }

    public class GetAllServicesQuery : IQuery<BaseResult<IEnumerable<ServiceGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllServicesHandler : IQueryHandler<GetAllServicesQuery, BaseResult<IEnumerable<ServiceGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllServicesHandler> _logger;

        public GetAllServicesHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllServicesHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<ServiceGetDto>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var services = _mapper.Map<IEnumerable<ServiceGetDto>>
                (await _unitOfWork.ServiceRepository.FindByQuery(null, false))
                ?? throw new NotFoundException($"Service not found");

                return BaseResult<IEnumerable<ServiceGetDto>>.Success(services);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllServicesHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
