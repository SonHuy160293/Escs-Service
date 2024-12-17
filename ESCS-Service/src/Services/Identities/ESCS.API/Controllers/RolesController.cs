using ESCS.Application.Features.Commands.Roles;
using ESCS.Application.Features.Queries.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {
            var roleQuery = new GetAllRolesQuery();

            var roleQueryResult = await _mediator.Send(roleQuery);
            return Ok(roleQueryResult.Data);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetRoleById(long roleId)
        {
            var getRoleByIdQuery = new GetRoleByIdQuery()
            {
                Id = roleId
            };

            var roleQueryResult = await _mediator.Send(getRoleByIdQuery);
            return Ok(roleQueryResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> PostRole(CreateRoleCommand createRoleCommand)
        {
            var roleCreatedResult = await _mediator.Send(createRoleCommand);

            return CreatedAtAction(nameof(PostRole), roleCreatedResult.Succeeded);
        }

    }
}
