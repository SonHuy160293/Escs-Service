using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Services
{
    internal class UpdateService
    {
    }
    public class UpdateServiceCommand : ICommand<BaseResult>
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? BaseUrl { get; set; }
    }

    public class UpdateServiceHandler : ICommandHandler<UpdateServiceCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateServiceHandler> _logger;

        public UpdateServiceHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateServiceHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetById(request.Id)
               ?? throw new NotFoundException($"Service with id:{request.Id} not found");


                service.Name = request.Name ?? service.Name;
                service.BaseUrl = request.BaseUrl ?? service.BaseUrl;

                _unitOfWork.ServiceRepository.Update(service);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(UpdateServiceHandler).Name, exceptionError);
                throw;
            }
        }
    }

}
