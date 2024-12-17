using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.ServiceEndpoints
{
    internal class UpdateServiceEndpoint
    {
    }

    public class UpdateServiceEndpointCommand : ICommand<BaseResult>
    {
        public long Id { get; set; }
        public string Method { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; }


    }

    public class UpdateServiceEndpointHandler : ICommandHandler<UpdateServiceEndpointCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateServiceEndpointHandler> _logger;


        public UpdateServiceEndpointHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<UpdateServiceEndpointHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult> Handle(UpdateServiceEndpointCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var serviceEndpoint = await _unitOfWork.ServiceEndpointRepository.GetById(request.Id);

                if (serviceEndpoint == null)
                {
                    throw new NotFoundException($"Endpoint with id:{request.Id} not found");
                }




                serviceEndpoint.Url = request.Url ?? serviceEndpoint.Url;
                serviceEndpoint.Method = request.Method ?? serviceEndpoint.Method;
                serviceEndpoint.IsActive = request.IsActive;
                serviceEndpoint.Description = request.Description ?? serviceEndpoint.Description;


                _unitOfWork.ServiceEndpointRepository.Update(serviceEndpoint);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(UpdateServiceEndpointHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
