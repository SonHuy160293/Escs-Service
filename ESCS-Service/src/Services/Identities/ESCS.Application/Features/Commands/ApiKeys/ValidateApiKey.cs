using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.ApiKeys
{
    class ValidateApiKey
    {
    }

    public class ValidateUserApiKeyCommand : ICommand<BaseResult<long>>
    {
        public string Key { get; set; }
        public string RequestPath { get; set; }
        public string Method { get; set; }

    }

    public class ValidateUserApiKeyHandler : ICommandHandler<ValidateUserApiKeyCommand, BaseResult<long>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ValidateUserApiKeyHandler> _logger;

        public ValidateUserApiKeyHandler(IUnitOfWork identityUnitOfWork, ILogger<ValidateUserApiKeyHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
        }

        //Handling validate api key
        public async Task<BaseResult<long>> Handle(ValidateUserApiKeyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //get key by key string
                var key = await _unitOfWork.UserApiKeyRepository
                    .FindEntityByQuery(k => k.Key == request.Key) ?? throw new NotFoundException("Key not found"); ;

                //check if key is allowed to endpoint
                var allowedEndpoint = await _unitOfWork.KeyAllowedEndpointRepository.
                    FindEntityByQuery(e => e.UserApiKeyId == key.Id && e.ServiceEndpoint.Url == request.RequestPath
                                      && e.ServiceEndpoint.Method.ToUpper() == request.Method && e.IsActive == true && e.UserApiKey.IsActive, false,
                                      e => e.ServiceEndpoint)
                    ?? throw new AuthenticationException("Not Allowed");

                return BaseResult<long>.Success(key.UserId);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(ValidateUserApiKeyCommand).Name, exceptionError);
                throw;
            }


        }
    }
}
