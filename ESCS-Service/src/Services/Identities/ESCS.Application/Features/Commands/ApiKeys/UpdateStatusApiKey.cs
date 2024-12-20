using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.ApiKeys
{
    internal class UpdateStatusApiKey
    {
    }

    public class UpdateUserApiKeyStatusCommand : ICommand<BaseResult<long>>
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }

    }

    public class UpdateUserApiKeyStatusHandler : ICommandHandler<UpdateUserApiKeyStatusCommand, BaseResult<long>>
    {

        private readonly ILogger<UpdateUserApiKeyStatusHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserApiKeyStatusHandler(IUnitOfWork identityUnitOfWork, ILogger<UpdateUserApiKeyStatusHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
        }


        //Handling update status of api key
        public async Task<BaseResult<long>> Handle(UpdateUserApiKeyStatusCommand request, CancellationToken cancellationToken)
        {

            try
            {
                //get key by id
                var key = await _unitOfWork.UserApiKeyRepository.FindEntityByQuery(k => k.Id == request.Id);

                //if key not exist return
                if (key is null)
                {
                    throw new NotFoundException("Key not found");
                }

                //set status
                key.IsActive = request.IsActive;

                //updte to db
                _unitOfWork.UserApiKeyRepository.Update(key);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult<long>.Success(key.Id);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(UpdateUserApiKeyStatusHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
