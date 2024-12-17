using ESCS.Application.Features.Commands.KeyAllowedEndpoints;
using ESCS.Application.Features.Commands.ServiceEndpoints;
using ESCS.Application.Features.Commands.Services;
using ESCS.Application.Features.Queries.KeyAllowedEndpoints;
using ESCS.Application.Features.Queries.ServiceEndpoints;
using ESCS.Application.Features.Queries.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostService(CreateServiceCommand createServiceCommand)
        {
            var serviceCreatedResult = await _mediator.Send(createServiceCommand);

            return CreatedAtAction(nameof(PostService), serviceCreatedResult.Succeeded);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService(UpdateServiceCommand updateServiceCommand)
        {
            var serviceUpdatedResult = await _mediator.Send(updateServiceCommand);
            return Ok(serviceUpdatedResult.Succeeded);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetAllService()
        {
            var serviceQuery = new GetAllServicesQuery();

            var serviceQueryResult = await _mediator.Send(serviceQuery);

            return Ok(serviceQueryResult.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(long id)
        {
            var getServiceByIdQuery = new GetServiceByIdQuery()
            {
                Id = id
            };

            var serviceQueryResult = await _mediator.Send(getServiceByIdQuery);

            return Ok(serviceQueryResult.Data);
        }

        [HttpPost("endpoint")]
        public async Task<IActionResult> PostEndpoint(CreateServiceEndpointCommand createServiceEndpointCommand)
        {
            var serviceEndpointCreatedResult = await _mediator.Send(createServiceEndpointCommand);

            return Ok(serviceEndpointCreatedResult.Succeeded);
        }

        [HttpPut("endpoint")]
        public async Task<IActionResult> UpdateEndpoint(UpdateServiceEndpointCommand updateServiceEndpointCommand)
        {
            var serviceEndpointUpdatedResult = await _mediator.Send(updateServiceEndpointCommand);
            return Ok(serviceEndpointUpdatedResult.Succeeded);
        }

        [HttpGet("endpoint/{id}")]
        public async Task<IActionResult> GetServiceEndpointById(long id)
        {
            var getServiceEndpointByIdQuery = new GetServiceEndpointByIdQuery()
            {
                Id = id
            };

            var serviceEndpointQueryResult = await _mediator.Send(getServiceEndpointByIdQuery);

            return Ok(serviceEndpointQueryResult.Data);
        }

        [HttpGet("endpoint")]
        public async Task<IActionResult> GetServiceEndpointByServiceId([FromQuery] long serviceId)
        {
            var getServiceEndpointByServiceIdQuery = new GetServiceEndpointByServiceIdQuery()
            {
                ServiceId = serviceId
            };

            var serviceEndpointQueryResult = await _mediator.Send(getServiceEndpointByServiceIdQuery);

            return Ok(serviceEndpointQueryResult.Data);
        }

        [HttpPost("endpoint/allowed")]
        public async Task<IActionResult> CreateKeyAllowedEndpoint(CreateKeyAllowedEndpointCommand createKeyAllowedEndpointCommand)
        {
            var createKeyAllowedEndpointCommandResult = await _mediator.Send(createKeyAllowedEndpointCommand);

            return CreatedAtAction(nameof(PostService), createKeyAllowedEndpointCommandResult.Succeeded);
        }

        [HttpPost("endpoint/list-allowed")]
        public async Task<IActionResult> CreateListKeyAllowedEndpoint(CreateKeyAllowedEndpointCommand createKeyAllowedEndpointCommand)
        {
            var createKeyAllowedEndpointCommandResult = await _mediator.Send(createKeyAllowedEndpointCommand);

            return Ok(createKeyAllowedEndpointCommandResult.Succeeded);
        }


        [HttpGet("endpoint/allowed/{keyId}")]
        public async Task<IActionResult> GetKeyAllowedEndpointByKey(long keyId)
        {
            var getKeyAllowedEndpointByKeyIdQuery = new GetKeyAllowedEndpointByKeyIdQuery()
            {
                KeyId = keyId
            };

            var keyAllowedEndpointQuery = await _mediator.Send(getKeyAllowedEndpointByKeyIdQuery);

            return Ok(keyAllowedEndpointQuery.Data);
        }

        [HttpGet("endpoint/allowed")]
        public async Task<IActionResult> GetKeyAllowedAllowedEndpoint()
        {
            var keyAllowedEndpointQuery = new GetAllKeyAllowedEndpointsQuery();

            var keyAllowedEndpointQueryResult = await _mediator.Send(keyAllowedEndpointQuery);

            return Ok(keyAllowedEndpointQueryResult.Data);
        }

        [HttpGet("email/user")]
        public async Task<IActionResult> GetUserInService([FromQuery] string? userName, [FromQuery] int? pageIndex, [FromQuery] int? pageSize)
        {


            var userSeachRequest = new Domain.Dtos.UserSearchRequest
            {
                PageSize = pageSize ?? 1,
                PageIndex = pageIndex ?? 0,
                UserName = userName ?? string.Empty
            };

            var getUserInEmailServiceWithPaginationQuery = new GetUserInEmailServiceWithPaginationQuery
            {
                UserSearchRequest = userSeachRequest,
            };

            var getUserInEmailServiceWithPaginationResult = await _mediator.Send(getUserInEmailServiceWithPaginationQuery);

            return Ok(getUserInEmailServiceWithPaginationResult);
        }

    }
}
