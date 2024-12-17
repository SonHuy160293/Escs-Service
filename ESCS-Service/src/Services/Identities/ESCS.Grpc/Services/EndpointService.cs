using ESCS.Application.Features.Queries.KeyAllowedEndpoints;
using ESCS.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace ESCS.Grpc.Services
{
    public class EndpointService : EndpointProtoService.EndpointProtoServiceBase
    {

        private readonly IMediator _mediator;
        public EndpointService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetUserRegisteredEndpointResponse> GetUserRegisteredEndpoint(GetUserRegisteredEndpointRequest request, ServerCallContext context)
        {
            try
            {
                var query = new GetUsersRegisterdInEndpointQuery
                {
                    Method = request.Method,
                    Url = request.Url,
                };

                var result = await _mediator.Send(query);

                var endpoints = result.Data;

                var users = endpoints.Select(e => new User
                {
                    Email = e.Email,
                    Id = e.UserId
                });

                // Create the response and add the user IDs to the UserId list
                var response = new GetUserRegisteredEndpointResponse();
                response.Users.AddRange(users);

                return response;
            }
            catch (Exception ex)
            {
                return default;
            }
        }


    }
}
