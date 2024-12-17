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

        public async Task<BaseResult> Handle(CreateEmailServiceConfigCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var service = await _unitOfWork.ServiceRepository.GetById(request.ServiceId);

                if (service is null)
                {
                    throw new NotFoundException($"Service with id:{request.ServiceId} is not found");
                }

                var email = await _unitOfWork.UserEmailServiceConfigurationRepository.FindEntityByQuery(e => e.SmtpEmail == request.SmtpEmail);

                if (email is not null)
                {
                    throw new ExistException($"{request.SmtpEmail} exist");
                }


                var emailServiceConfig = _mapper.Map<UserEmailServiceConfiguration>(request);
                emailServiceConfig.IsActive = true;

                await _unitOfWork.UserEmailServiceConfigurationRepository.Add(emailServiceConfig);

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
