using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.ApiKeys
{
    internal class CreateApiKey
    {
    }

    public class CreateUserApiKeyCommand : ICommand<BaseResult<long>>
    {
        public long UserId { get; set; }
        public long ServiceId { get; set; }

    }

    public class CreateUserApiKeyHandler : ICommandHandler<CreateUserApiKeyCommand, BaseResult<long>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateUserApiKeyHandler> _logger;

        public CreateUserApiKeyHandler(IUnitOfWork identityUnitOfWork, ILogger<CreateUserApiKeyHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _logger = logger;
        }



        public async Task<BaseResult<long>> Handle(CreateUserApiKeyCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var user = await _unitOfWork.UserRepository.GetById(request.UserId);

                if (user is null)
                {
                    throw new NotFoundException("User not found");
                }


                var secureKey = Core.Application.Extensions.Extension.GenerateSecureApiKey();

                var userApiKey = new UserApiKey
                {
                    ServiceId = request.ServiceId,
                    Key = secureKey,
                    UserId = request.UserId,
                    IsActive = true
                };

                await _unitOfWork.UserApiKeyRepository.Add(userApiKey);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult<long>.Success(userApiKey.Id);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateUserApiKeyHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
