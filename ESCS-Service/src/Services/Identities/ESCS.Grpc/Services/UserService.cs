using ESCS.Application.Features.Commands.ApiKeys;
using ESCS.Application.Features.Queries.UserEmailServiceConfigs;
using ESCS.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace ESCS.Grpc.Services
{
    public class UserService : UserProtoService.UserProtoServiceBase
    {
        private readonly IMediator _mediator;

        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Implementation for ValidateUserApiKey RPC
        public override async Task<ValidateUserApiKeyResponse> ValidateUserApiKey(ValidateUserApiKeyRequest request, ServerCallContext context)
        {
            try
            {
                // Use Mediator to send the command
                var command = new ValidateUserApiKeyCommand
                {
                    Key = request.Key,
                    RequestPath = request.RequestPath,
                    Method = request.Method
                };

                var result = await _mediator.Send(command);

                // Map result to gRPC response
                return new ValidateUserApiKeyResponse
                {
                    UserId = result.Data
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Implementation for GetUserEmailConfigByEmail RPC
        public override async Task<GetUserEmailConfigByEmailResponse> GetUserEmailConfigByEmail(GetUserEmailConfigByEmailRequest request, ServerCallContext context)
        {
            try
            {
                // Use Mediator to send the query
                var query = new GetEmailServiceConfigDetailByEmailQuery
                {
                    Email = request.Email
                };

                var result = await _mediator.Send(query);

                // Map result to gRPC response
                var userEmailConfig = result.Data;
                return new GetUserEmailConfigByEmailResponse
                {
                    UserEmailConfig = new UserEmailConfig
                    {
                        Id = (long)userEmailConfig.Id,
                        SmtpEmail = userEmailConfig.SmtpEmail,
                        SmtpPort = userEmailConfig.SmtpPort,
                        SmtpServer = userEmailConfig.SmtpServer,
                        SmtpPassword = userEmailConfig.SmtpPassword,
                        UserId = userEmailConfig.UserId,
                        ServiceId = userEmailConfig.ServiceId,
                        IsActive = userEmailConfig.IsActive,
                        IsEnableSsl = userEmailConfig.IsEnableSsl,
                    }
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}