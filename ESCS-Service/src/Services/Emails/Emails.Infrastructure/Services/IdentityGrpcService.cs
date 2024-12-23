using Core.Application.Exceptions;
using Emails.Application.Services;
using ESCS.Grpc.Protos;
using Grpc.Net.Client;
using UserEmailConfig = UserEmailConfiguration.Cache.Models.UserEmailConfig;

namespace Emails.Infrastructure.Services
{
    public class IdentityGrpcService : IIdentityGrpcService
    {

        private readonly UserProtoService.UserProtoServiceClient _client;

        public IdentityGrpcService()
        {
            // Replace "http://localhost:5026" with the actual gRPC server address.
            var channel = GrpcChannel.ForAddress("http://localhost:5026");
            _client = new UserProtoService.UserProtoServiceClient(channel);
        }

        public async Task<UserEmailConfig> GetUserEmailConfig(string smtpEmail)
        {
            var request = new GetUserEmailConfigByEmailRequest
            {
                Email = smtpEmail
            };

            var response = await _client.GetUserEmailConfigByEmailAsync(request);

            if (response is null)
            {
                throw new AuthenticationException("Email not valid"); // Handle the error accordingly
            }

            return new UserEmailConfig
            {
                Id = response.UserEmailConfig.Id,
                SmtpEmail = response.UserEmailConfig.SmtpEmail,
                SmtpPort = response.UserEmailConfig.SmtpPort,
                SmtpPassword = response.UserEmailConfig.SmtpPassword,
                UserId = response.UserEmailConfig.UserId,
                ServiceId = response.UserEmailConfig.ServiceId,
                SmtpServer = response.UserEmailConfig.SmtpServer,
                IsEnableSsl = response.UserEmailConfig.IsEnableSsl,
            };
        }

        public async Task<long> ValidateUserApiKey(string apiKey, string requestPath, string method)
        {
            var request = new ValidateUserApiKeyRequest
            {
                Key = apiKey,
                RequestPath = requestPath,
                Method = method
            };

            var response = await _client.ValidateUserApiKeyAsync(request);

            if (response is null)
            {
                throw new AuthenticationException("Failed to validate API key."); // Handle the error accordingly
            }
            return response.UserId;
        }
    }
}

