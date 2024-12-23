using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Identity.Cache.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.KeyAllowedEndpoints
{
    internal class UpdateEndpointOfKey
    {
    }

    public class UpdateEndpointOfKeyCommand : ICommand<BaseResult>
    {
        public long KeyId { get; set; }
        public List<long> EndpointsId { get; set; }

    }

    public class UpdateEndpointOfKeyHandler : ICommandHandler<UpdateEndpointOfKeyCommand, BaseResult>
    {

        private readonly ILogger<UpdateEndpointOfKeyHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachedEndpointUserRepository _cachedEndpointUserRepository;


        public UpdateEndpointOfKeyHandler(IUnitOfWork identityUnitOfWork, ILogger<UpdateEndpointOfKeyHandler> logger, ICachedEndpointUserRepository cachedEndpointUserRepository)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _cachedEndpointUserRepository = cachedEndpointUserRepository;
        }


        //Handling update status of api key
        public async Task<BaseResult> Handle(UpdateEndpointOfKeyCommand request, CancellationToken cancellationToken)
        {

            try
            {
                //get key by id
                var key = await _unitOfWork.UserApiKeyRepository.FindEntityByQuery(k => k.Id == request.KeyId);

                //if key not exist return
                if (key is null)
                {
                    throw new NotFoundException("Key not found");
                }

                var allowedEndpointsOfKey = await _unitOfWork.KeyAllowedEndpointRepository.FindByQuery(k => k.UserApiKeyId == request.KeyId, false) ?? throw new NotFoundException("No endpoint to delete");

                _unitOfWork.KeyAllowedEndpointRepository.DeleteRange(allowedEndpointsOfKey);

                //get endpoints by Id from db
                var endpoints = await _unitOfWork.ServiceEndpointRepository.FindByQuery(e => request.EndpointsId.Contains(e.Id));

                //if > 1 endpoint not exist return
                if (endpoints.Count() != request.EndpointsId.Count())
                {
                    throw new NotFoundException("Endpoint not found");
                }

                var keyAllowedList = new List<KeyAllowedEndpoint>();

                foreach (var endpoint in endpoints)
                {

                    //check if endpoint is saved in redis
                    var endpointUser = await _cachedEndpointUserRepository.GetEndpointUser(endpoint.Url, endpoint.Method);

                    //if endpoint is exists in redis then remove
                    if (endpointUser is not null)
                    {
                        await _cachedEndpointUserRepository.DeleteEndpointUser(endpoint.Url, endpoint.Method);
                    }

                    //create key allowed object
                    var keyAllowedEndpoint = new KeyAllowedEndpoint
                    {
                        UserApiKeyId = request.KeyId,
                        EndpointId = endpoint.Id,
                        IsActive = true,

                    };

                    //add object to list
                    keyAllowedList.Add(keyAllowedEndpoint);

                }

                //add list allowed to db
                await _unitOfWork.KeyAllowedEndpointRepository.AddRange(keyAllowedList);

                //save change
                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(UpdateEndpointOfKeyHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
