using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Identity.Cache.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.KeyAllowedEndpoints
{
    internal class CreateKeyAllowedEndpoint
    {
    }


    public class CreateKeyAllowedEndpointCommand : ICommand<BaseResult>
    {
        public long UserApiKeyId { get; set; }
        public List<long> EndpointId { get; set; }

    }

    public class CreateUserApiKeyHandler : ICommandHandler<CreateKeyAllowedEndpointCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachedEndpointUserRepository _cachedEndpointUserRepository;
        private readonly ILogger<CreateUserApiKeyHandler> _logger;
        public CreateUserApiKeyHandler(IUnitOfWork identityUnitOfWork, ILogger<CreateUserApiKeyHandler> logger
            , ICachedEndpointUserRepository cachedEndpointUserRepository)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _cachedEndpointUserRepository = cachedEndpointUserRepository;
        }

        public async Task<BaseResult> Handle(CreateKeyAllowedEndpointCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var key = await _unitOfWork.UserApiKeyRepository.GetById(request.UserApiKeyId);

                if (key is null)
                {
                    throw new NotFoundException("Key not found");
                }

                var endpoints = await _unitOfWork.ServiceEndpointRepository.FindByQuery(e => request.EndpointId.Contains(e.Id));

                if (endpoints.Count() != request.EndpointId.Count())
                {
                    throw new NotFoundException("Endpoint not found");
                }

                var keyAllowedList = new List<KeyAllowedEndpoint>();

                foreach (var endpoint in endpoints)
                {

                    var endpointUser = await _cachedEndpointUserRepository.GetEndpointUser(endpoint.Url, endpoint.Method);

                    if (endpointUser is not null)
                    {
                        await _cachedEndpointUserRepository.DeleteEndpointUser(endpoint.Url, endpoint.Method);
                    }

                    var keyAllowedEndpoint = new KeyAllowedEndpoint
                    {
                        UserApiKeyId = request.UserApiKeyId,
                        EndpointId = endpoint.Id,
                        IsActive = true,

                    };
                    keyAllowedList.Add(keyAllowedEndpoint);

                }

                await _unitOfWork.KeyAllowedEndpointRepository.AddRange(keyAllowedList);

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
