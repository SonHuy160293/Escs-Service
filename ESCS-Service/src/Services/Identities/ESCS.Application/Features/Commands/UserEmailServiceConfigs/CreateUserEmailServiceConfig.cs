using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.UserEmailServiceConfigs
{
    public class CreateEmailServiceConfigCommand : ICommand<BaseResult>
    {
        public string SmtpEmail { get; set; } = default!;
        public string SmtpPassword { get; set; } = default!;
        public int SmtpPort { get; set; }

        public long UserId { get; set; }

        public long ServiceId { get; set; }


    }

    public class CreateEmailServiceConfigHandler : ICommandHandler<CreateEmailServiceConfigCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmailServiceConfigHandler> _logger;

        public CreateEmailServiceConfigHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<CreateEmailServiceConfigHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        //Handling create user email config
        public async Task<BaseResult> Handle(CreateEmailServiceConfigCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //get service by Id
                var service = await _unitOfWork.ServiceRepository.GetById(request.ServiceId);

                //if service not exist then return
                if (service is null)
                {
                    throw new NotFoundException($"Service with id:{request.ServiceId} is not found");
                }

                //check if email is used or not
                var email = await _unitOfWork.UserEmailServiceConfigurationRepository.FindEntityByQuery(e => e.SmtpEmail == request.SmtpEmail);

                //if exist then return
                if (email is not null)
                {
                    throw new ExistException($"{request.SmtpEmail} exist");
                }

                //mapping
                var emailServiceConfig = _mapper.Map<UserEmailServiceConfiguration>(request);

                //set default status to true
                emailServiceConfig.IsActive = true;

                //add object
                await _unitOfWork.UserEmailServiceConfigurationRepository.Add(emailServiceConfig);

                //save change
                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateEmailServiceConfigHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
