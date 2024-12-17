using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Application.Features.Commands.ServiceEndpoints;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using UserEmailConfiguration.Cache.Interfaces;

namespace ESCS.Application.Features.Commands.UserEmailServiceConfigs
{
    internal class UpdateUserEmailServiceConfig
    {
    }

    public class UpdateEmailServiceConfigCommand : ICommand<BaseResult>
    {
        public long Id { get; set; }
        public string SmtpEmail { get; set; } = default!;
        public string SmtpPassword { get; set; } = default!;
        public int SmtpPort { get; set; }
        public bool IsActive { get; set; }


    }

    public class UpdateEmailServiceConfigHandler : ICommandHandler<UpdateEmailServiceConfigCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateServiceEndpointHandler> _logger;
        private readonly ICachedUserEmailConfigRepository _cachedUserEmailConfigRepository;

        public UpdateEmailServiceConfigHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<UpdateServiceEndpointHandler> logger,
            ICachedUserEmailConfigRepository cachedUserEmailConfigRepository)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _cachedUserEmailConfigRepository = cachedUserEmailConfigRepository;
        }

        public async Task<BaseResult> Handle(UpdateEmailServiceConfigCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var emailServiceConfig = await _unitOfWork.UserEmailServiceConfigurationRepository.GetById(request.Id);

                var cacheUserEmailConfig = await _cachedUserEmailConfigRepository.GetUserEmailConfig(emailServiceConfig.SmtpEmail);
                if (cacheUserEmailConfig != null)
                {
                    await _cachedUserEmailConfigRepository.DeleteUserEmailConfig(emailServiceConfig.SmtpEmail);
                }

                if (emailServiceConfig is null)
                {
                    throw new NotFoundException($"Config with id:{request.Id} not exist");
                }


                emailServiceConfig.SmtpPort = request.SmtpPort;
                emailServiceConfig.SmtpEmail = request.SmtpEmail;
                emailServiceConfig.SmtpPassword = request.SmtpPassword;
                emailServiceConfig.IsActive = request.IsActive;

                _unitOfWork.UserEmailServiceConfigurationRepository.Update(emailServiceConfig);

                await _unitOfWork.SaveChangesAsync();



                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(UpdateEmailServiceConfigHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
