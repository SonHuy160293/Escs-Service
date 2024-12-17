using ESCS.Application.Features.Commands.Authentications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {

        private readonly IMediator _mediator;
        public AuthenticationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand loginCommand)
        {
            var loginResult = await _mediator.Send(loginCommand);
            return Ok(loginResult.Data);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand logoutCommand)
        {
            var logoutResult = await _mediator.Send(logoutCommand);
            return Ok(logoutResult.Succeeded);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommand refreshTokenCommand)
        {
            var refreshTokenResult = await _mediator.Send(refreshTokenCommand);
            return Ok(refreshTokenResult.Data);
        }


    }
}
