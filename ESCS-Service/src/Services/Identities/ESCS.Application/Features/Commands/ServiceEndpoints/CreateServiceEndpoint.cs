using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.ServiceEndpoints
{
    internal class CreateServiceEndpoint
    {
    }

    public class CreateServiceEndpointCommand : ICommand<BaseResult>
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public long ServiceId { get; set; }
    }

    public class CreateServiceEndpointHandler : ICommandHandler<CreateServiceEndpointCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateServiceEndpointHandler> _logger;

        public CreateServiceEndpointHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<CreateServiceEndpointHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult> Handle(CreateServiceEndpointCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var service = await _unitOfWork.ServiceRepository.GetById(request.ServiceId);

                if (service is null)
                {
                    throw new NotFoundException($"Service with id:{request.ServiceId} not found");
                }

                var serviceEndpoint = await _unitOfWork.ServiceEndpointRepository.FindEntityByQuery(e => e.Url == request.Url && e.Method == request.Method);

                if (serviceEndpoint is not null)
                {
                    throw new ExistException("Endpoint alrealdy existed");
                }

                serviceEndpoint = _mapper.Map<ServiceEndpoint>(request);

                serviceEndpoint.IsActive = true;

                await _unitOfWork.ServiceEndpointRepository.Add(serviceEndpoint);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateServiceEndpointHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
