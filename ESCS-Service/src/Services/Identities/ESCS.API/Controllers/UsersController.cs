using ESCS.Application.Features.Commands.ApiKeys;
using ESCS.Application.Features.Commands.KeyAllowedEndpoints;
using ESCS.Application.Features.Commands.UserEmailServiceConfigs;
using ESCS.Application.Features.Commands.Users;
using ESCS.Application.Features.Queries.ApiKeys;
using ESCS.Application.Features.Queries.KeyAllowedEndpoints;
using ESCS.Application.Features.Queries.UserEmailServiceConfigs;
using ESCS.Application.Features.Queries.Users;
using ESCS.Application.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(CreateUserCommand createUserCommand)
        {
            var userCreatedResult = await _mediator.Send(createUserCommand);

            return CreatedAtAction(nameof(PostUser), userCreatedResult.Succeeded);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserCommand updateUserCommand)
        {
            var userUpdatedResult = await _mediator.Send(updateUserCommand);
            return Ok(userUpdatedResult.Succeeded);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var userQuery = new GetAllUsersQuery();

            var userQueryResult = await _mediator.Send(userQuery);

            return Ok(userQueryResult.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var getUserByIdQuery = new GetUserByIdQuery()
            {
                Id = id
            };

            var userQueryResult = await _mediator.Send(getUserByIdQuery);

            return Ok(userQueryResult.Data);
        }


        [HttpPost("api-key")]
        public async Task<IActionResult> CreateUserApiKey(CreateUserApiKeyCommand createUserApiKeyCommand)
        {
            var userApiKeyCreatedResult = await _mediator.Send(createUserApiKeyCommand);
            return Ok(userApiKeyCreatedResult.Data);
        }

        [HttpPost("api-key-allowed-endpoint")]
        public async Task<IActionResult> CreateApiKeyAllowedEndpoint(CreateApiKeyAllowedEndpointTransactionCommand createUserApiKeyCommand)
        {
            var userApiKeyCreatedResult = await _mediator.Send(createUserApiKeyCommand);
            return Ok(userApiKeyCreatedResult.Succeeded);
        }

        [HttpGet("api-key")]
        public async Task<IActionResult> GetAllUserApiKey()
        {
            var userApiKeyQuery = new GetAllUserApiKeysQuery();

            var userApiKeyQueryResult = await _mediator.Send(userApiKeyQuery);
            return Ok(userApiKeyQueryResult.Data);
        }

        [HttpGet("api-key/{userId}")]
        public async Task<IActionResult> GetUserApiKeyByUserId(long userId)
        {
            var getApiKeyByUserIdQuery = new GetApiKeyByUserIdQuery()
            {
                UserId = userId
            };

            var userApiKeyQueryResult = await _mediator.Send(getApiKeyByUserIdQuery);
            return Ok(userApiKeyQueryResult.Data);
        }

        [HttpGet("api-key/user/{userId}/service/{serviceId}")]
        public async Task<IActionResult> GetUserApiKeyByUserIdAndServiceID(long userId, long serviceId)
        {
            var getApiKeyByUserIdAndServiceQuery = new GetApiKeyByUserIdAndServiceIdQuery()
            {
                UserId = userId,
                ServiceId = serviceId

            };

            var userApiKeyQueryResult = await _mediator.Send(getApiKeyByUserIdAndServiceQuery);
            return Ok(userApiKeyQueryResult.Data);
        }


        [HttpGet("api-key/detail")]
        public async Task<IActionResult> GetUserApiKeyDetailByUserId([FromQuery] long userId, [FromQuery] long serviceId)
        {
            var getApiKeyDetailByUserIdQuery = new GetUserApiKeyDetailByUserIdQuery()
            {
                UserId = userId,
                ServiceId = serviceId
            };

            var userApiKeyQueryResult = await _mediator.Send(getApiKeyDetailByUserIdQuery);
            return Ok(userApiKeyQueryResult.Data);
        }

        [HttpGet("api-key/{id}/detail")]
        public async Task<IActionResult> GetUserApiKeyDetailById(long id)
        {
            var getUserApiKeyDetailByIdQuery = new GetUserApiKeyDetailByIdQuery()
            {
                Id = id
            };

            var getUserApiKeyDetailByIdQueryResult = await _mediator.Send(getUserApiKeyDetailByIdQuery);
            return Ok(getUserApiKeyDetailByIdQueryResult.Data);
        }

        [HttpPut("api-key")]
        public async Task<IActionResult> UpdateApiKeyStatus(UpdateUserApiKeyStatusCommand updateUserApiKeyStatusCommand)
        {

            var updateUserApiKeyStatusResult = await _mediator.Send(updateUserApiKeyStatusCommand);
            return Ok(updateUserApiKeyStatusResult.Data);
        }

        [HttpPut("api-key-allowed")]
        public async Task<IActionResult> UpdateApiKeyAllowed(UpdateEndpointOfKeyCommand updateEndpointOfKeyCommand)
        {

            var updateEndpointOfKeyResult = await _mediator.Send(updateEndpointOfKeyCommand);
            return Ok(updateEndpointOfKeyResult);
        }


        [HttpPost("email-config")]
        public async Task<IActionResult> PostEmailServiceConfig(CreateEmailServiceConfigCommand createEmailServiceConfigCommand)
        {
            var createEmailServiceConfigResult = await _mediator.Send(createEmailServiceConfigCommand);
            return Ok(createEmailServiceConfigResult.Succeeded);

        }


        [HttpPut("email-config")]
        public async Task<IActionResult> PutEmailServiceConfig(UpdateEmailServiceConfigCommand updateEmailServiceConfigCommand)
        {
            var updateEmailServiceConfigResult = await _mediator.Send(updateEmailServiceConfigCommand);
            return Ok(updateEmailServiceConfigResult.Succeeded);

        }

        [HttpGet("email-config")]
        public async Task<IActionResult> GetEmailServiceConfig()
        {
            var emailServiceConfigQuery = new GetAllEmailServiceConfigsQuery();

            var emailServiceConfigQueryResult = await _mediator.Send(emailServiceConfigQuery);
            return Ok(emailServiceConfigQueryResult.Data);
        }



        [HttpGet("email-config/{userId}")]
        public async Task<IActionResult> GetEmailServiceConfigByUserId(long userId)
        {
            var getEmailServiceConfigByUserIdQuery = new GetEmailServiceConfigByUserIdQuery()
            {
                UserId = userId
            };

            var emailServiceConfigQueryResult = await _mediator.Send(getEmailServiceConfigByUserIdQuery);
            return Ok(emailServiceConfigQueryResult.Data);
        }

        [HttpGet("email-config/detail")]
        public async Task<IActionResult> GetEmailServiceConfigByEmail([FromQuery] string email)
        {
            var getEmailServiceConfigByEmailQuery = new GetEmailServiceConfigDetailByEmailQuery()
            {
                Email = email
            };

            var emailServiceConfigQueryResult = await _mediator.Send(getEmailServiceConfigByEmailQuery);
            return Ok(emailServiceConfigQueryResult.Data);
        }

        [HttpGet("{userId}/email-config/detail")]
        [ServiceFilter(typeof(ApiKeyAuthorizationFilter))]
        public async Task<IActionResult> GetEmailServiceConfigDetailByUserId(long userId)
        {
            var getEmailServiceConfigDetailByUserIdQuery = new GetEmailServiceConfigDetailByUserIdQuery()
            {
                UserId = userId
            };

            var emailServiceConfigDetailQueryResult = await _mediator.Send(getEmailServiceConfigDetailByUserIdQuery);
            return Ok(emailServiceConfigDetailQueryResult.Data);
        }

        [HttpGet("{userId}/service-registered")]
        public async Task<IActionResult> GetServiceRegiterByUser(long userId)
        {
            var getServiceEndpointRegisteredByUserIdQuery = new GetServiceEndpointRegisteredByUserIdQuery()
            {
                UserId = userId
            };

            var getServiceEndpointRegisteredByUserIdResult = await _mediator.Send(getServiceEndpointRegisteredByUserIdQuery);
            return Ok(getServiceEndpointRegisteredByUserIdResult.Data);
        }

        [HttpGet("email-config/detail/{id}")]
        [ServiceFilter(typeof(ApiKeyAuthorizationFilter))]
        public async Task<IActionResult> GetEmailServiceConfigDetailById(long id)
        {
            var getEmailServiceConfigDetailByIdQuery = new GetEmailServiceConfigDetailByIdQuery()
            {
                Id = id
            };

            var getEmailServiceConfigDetailByIdResult = await _mediator.Send(getEmailServiceConfigDetailByIdQuery);
            return Ok(getEmailServiceConfigDetailByIdResult.Data);
        }


        [HttpPost("validate-key")]
        public async Task<IActionResult> ValidateToken(ValidateUserApiKeyCommand validateUserApiKeyCommand)
        {
            var validateResult = await _mediator.Send(validateUserApiKeyCommand);
            return Ok(validateResult.Data);
        }



    }
}
