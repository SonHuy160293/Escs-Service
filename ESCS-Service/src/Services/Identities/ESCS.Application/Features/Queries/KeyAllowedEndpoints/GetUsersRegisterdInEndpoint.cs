using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.KeyAllowedEndpoints
{
    class GetUsersRegisterdInEndpoint
    {
    }

    public class GetUsersRegisterdInEndpointQuery : IQuery<BaseResult<IEnumerable<UsersRegisterInEndpointGetDto>>>
    {
        public string Url { get; set; }
        public string Method { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetUsersRegisterdInEndpointHandler : IQueryHandler<GetUsersRegisterdInEndpointQuery, BaseResult<IEnumerable<UsersRegisterInEndpointGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersRegisterdInEndpointHandler> _logger;

        public GetUsersRegisterdInEndpointHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetUsersRegisterdInEndpointHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<UsersRegisterInEndpointGetDto>>> Handle(GetUsersRegisterdInEndpointQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var keyAllowedEndpoints = (await _unitOfWork.KeyAllowedEndpointRepository
                                  .FindByQuery(k => k.ServiceEndpoint.Url.Equals(request.Url) && k.ServiceEndpoint.Method.Equals(request.Method)
                                              , false, k => k.ServiceEndpoint, k => k.UserApiKey, k => k.UserApiKey.User))

                ?? throw new NotFoundException($"Endpoint with url:{request.Url} not found");

                var result = keyAllowedEndpoints.Select(k => new UsersRegisterInEndpointGetDto
                {
                    UserId = k.UserApiKey.UserId,
                    Email = k.UserApiKey.User.Name

                }).DistinctBy(se => se.UserId);

                return BaseResult<IEnumerable<UsersRegisterInEndpointGetDto>>.Success(result);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetUsersRegisterdInEndpointHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
