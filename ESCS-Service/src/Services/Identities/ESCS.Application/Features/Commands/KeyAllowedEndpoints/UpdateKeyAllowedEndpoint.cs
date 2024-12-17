using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.KeyAllowedEndpoints
{
    internal class UpdateKeyAllowedEndpoint
    {
    }

    public class UpdateKeyAllowedEndpointCommand : ICommand<BaseResult>
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }

    }

    public class UpdateUserApiKeyHandler : ICommandHandler<UpdateKeyAllowedEndpointCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserApiKeyHandler> _logger;

        public UpdateUserApiKeyHandler(IUnitOfWork identityUnitOfWork, ILogger<UpdateUserApiKeyHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _logger = logger;
        }

        public async Task<BaseResult> Handle(UpdateKeyAllowedEndpointCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var key = await _unitOfWork.KeyAllowedEndpointRepository.GetById(request.Id);

                if (key is null)
                {
                    throw new NotFoundException("Key not found");
                }

                key.IsActive = request.IsActive;


                _unitOfWork.KeyAllowedEndpointRepository.Update(key);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
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
