using Core.Application.Common;
using ESCS.Grpc.Protos;
using Grpc.Net.Client;
using Logs.API.Interfaces;

namespace Logs.API.Services
{
    public class IdentityGrpcService : IIdentityGrpcService
    {

        private readonly EndpointProtoService.EndpointProtoServiceClient _endpoint;
        private readonly ILogger<IdentityGrpcService> _logger;


        public IdentityGrpcService(ILogger<IdentityGrpcService> logger)
        {
            // Replace "http://localhost:5026" with the actual gRPC server address.
            var channel = GrpcChannel.ForAddress("http://localhost:5026");
            _logger = logger;
            _endpoint = new EndpointProtoService.EndpointProtoServiceClient(channel);
        }

        public async Task<IEnumerable<UserDto>> GetUserInServiceEndpoint(string url, string method)
        {
            try
            {
                var getUserRegisteredEndpointRequest = new GetUserRegisteredEndpointRequest { Url = url, Method = method };

                var response = await _endpoint.GetUserRegisteredEndpointAsync(getUserRegisteredEndpointRequest);

                var users = response.Users.Select(u => new UserDto
                {
                    Email = u.Email,
                    Id = u.Id,
                });

                return users;
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogError("GetUserInServiceEndpoint catch exception:{Exception}", exception);
                throw;
            }




        }

        public class UserDto
        {
            public long Id { get; set; }
            public string Email { get; set; }
        }
    }
}