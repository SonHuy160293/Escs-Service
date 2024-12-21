using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Services
{
    internal class CreateService
    {
    }
    public class CreateServiceCommand : ICommand<BaseResult>
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
    }

    //Handling create service
    public class CreateServiceHandler : ICommandHandler<CreateServiceCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateServiceHandler> _logger;

        public CreateServiceHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateServiceHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {

            try
            {
                //mapping
                var service = _mapper.Map<Service>(request);

                //add object to db
                await _unitOfWork.ServiceRepository.Add(service);

                //save change
                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateServiceHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
