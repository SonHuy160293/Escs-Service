using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Services
{
    internal class GetServiceById
    {
    }

    public class GetServiceByIdQuery : IQuery<BaseResult<ServiceGetDto>>
    {
        public long Id { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetServiceByIdHandler : IQueryHandler<GetServiceByIdQuery, BaseResult<ServiceGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetServiceByIdHandler> _logger;

        public GetServiceByIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetServiceByIdHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<ServiceGetDto>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var service = _mapper.Map<ServiceGetDto>
               (await _unitOfWork.ServiceRepository.FindEntityByQuery(s => s.Id == request.Id, false))
               ?? throw new NotFoundException($"Service with Id:{request.Id} not found");

                return BaseResult<ServiceGetDto>.Success(service);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetServiceByIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
