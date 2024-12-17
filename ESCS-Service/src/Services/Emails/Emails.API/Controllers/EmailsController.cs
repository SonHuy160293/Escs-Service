using Emails.API.Filters;
using Emails.Application.Features.Emails;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Emails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmailsController> _logger;

        public EmailsController(IMediator mediator, ILogger<EmailsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [ServiceFilter(typeof(ValidateSendMailRequestFilter))]
        [HttpPost]
        public async Task<IActionResult> SendMail([FromForm] SendEmailCommand sendEmailCommand)
        {

            var sendMailResult = await _mediator.Send(sendEmailCommand);

            return Ok(sendMailResult.Succeeded);
        }
    }
}
