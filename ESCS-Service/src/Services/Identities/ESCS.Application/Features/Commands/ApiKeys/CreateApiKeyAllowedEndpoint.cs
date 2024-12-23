using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Identity.Cache.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.ApiKeys
{
    internal class CreateApiKeyAllowedEndpointTransaction
    {
    }


    public class CreateApiKeyAllowedEndpointTransactionCommand : ICommand<BaseResult>
    {
        public long UserId { get; set; }
        public long ServiceId { get; set; }

        public List<long> EndpointId { get; set; }

    }

    public class CreateApiKeyAllowedEndpointTransactionHandler : ICommandHandler<CreateApiKeyAllowedEndpointTransactionCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateApiKeyAllowedEndpointTransactionHandler> _logger;
        private readonly ICachedEndpointUserRepository _cachedEndpointUserRepository;


        public CreateApiKeyAllowedEndpointTransactionHandler(IUnitOfWork identityUnitOfWork, ILogger<CreateApiKeyAllowedEndpointTransactionHandler> logger,
            ICachedEndpointUserRepository cachedEndpointUserRepository)
        {
            _unitOfWork = identityUnitOfWork;
            _logger = logger;
            _cachedEndpointUserRepository = cachedEndpointUserRepository;
        }



        //handling create user api key
        public async Task<BaseResult> Handle(CreateApiKeyAllowedEndpointTransactionCommand request, CancellationToken cancellationToken)
        {

            try
            {

                await _unitOfWork.BeginTransactionAsync();

                //get user by id
                var user = await _unitOfWork.UserRepository.GetById(request.UserId);

                //check if user is exists
                if (user is null)
                {
                    throw new NotFoundException("User not found");
                }

                //generate key
                var secureKey = Core.Application.Extensions.Extension.GenerateSecureApiKey();

                //create key object
                var userApiKey = new UserApiKey
                {
                    ServiceId = request.ServiceId,
                    Key = secureKey,
                    UserId = request.UserId,
                    IsActive = true
                };

                //add key to db
                await _unitOfWork.UserApiKeyRepository.Add(userApiKey);
                await _unitOfWork.SaveChangesAsync();


                //get endpoints by Id from db
                var endpoints = await _unitOfWork.ServiceEndpointRepository.FindByQuery(e => request.EndpointId.Contains(e.Id));

                //if > 1 endpoint not exist return
                if (endpoints.Count() != request.EndpointId.Count())
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
                        UserApiKeyId = userApiKey.Id,
                        EndpointId = endpoint.Id,
                        IsActive = true,

                    };

                    //add object to list
                    keyAllowedList.Add(keyAllowedEndpoint);

                }

                await _unitOfWork.KeyAllowedEndpointRepository.AddRange(keyAllowedList);

                await _unitOfWork.CommitTransactionAsync();



                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateApiKeyAllowedEndpointTransactionHandler).Name, exceptionError);


                throw;
            }
        }
    }
}
