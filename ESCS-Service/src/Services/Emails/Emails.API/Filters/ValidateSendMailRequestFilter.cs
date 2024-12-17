using Core.Application.Helpers;
using Emails.Application.Features.Emails;
using Emails.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserEmailConfiguration.Cache.Interfaces;

namespace Emails.API.Filters
{
    public class ValidateSendMailRequestFilter : IActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ValidateSendMailRequestFilter> _logger;
        private readonly ICachedUserEmailConfigRepository _cachedUserEmailConfigRepository;
        private readonly IIdentityService _identityService;
        private readonly IIdentityGrpcService _identityGrpcService;
        private readonly string[] _extensions = new string[]
        {
            ".pdf", ".doc", ".docx", ".ppt", ".png", ".jpeg", ".jpg", ".txt", ".zip"
        };

        public ValidateSendMailRequestFilter(IConfiguration configuration, ILogger<ValidateSendMailRequestFilter> logger,
            ICachedUserEmailConfigRepository cachedUserEmailConfigRepository, IIdentityService identityService, IIdentityGrpcService identityGrpcService)
        {
            _configuration = configuration;
            _logger = logger;
            _cachedUserEmailConfigRepository = cachedUserEmailConfigRepository;
            _identityService = identityService;
            _identityGrpcService = identityGrpcService;
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.ActionArguments.TryGetValue("sendEmailCommand", out var value) && value is SendEmailCommand request)
            {

                _logger.LogInformation("Validating request");

                // Check ObjectId
                if (string.IsNullOrEmpty(request.ObjectId))
                {
                    context.Result = new BadRequestObjectResult("ObjectId is required.");
                    return;
                }

                // Validate 'From' email if provided, or use default if missing
                if (!ValidationHelper.IsEmail(request.From))
                {
                    context.Result = new BadRequestObjectResult("Sender email address is invalid.");
                    return;
                }

                var userEmailConfig = await _cachedUserEmailConfigRepository.GetUserEmailConfig(request.From);

                if (userEmailConfig is null)
                {
                    //userEmailConfig = await _identityService.GetUserEmailConfig(request.From);

                    userEmailConfig = await _identityGrpcService.GetUserEmailConfig(request.From);

                    await _cachedUserEmailConfigRepository.AddUserEmailConfig(userEmailConfig);
                }


                // Validate 'To' email addresses
                if (request.To is null || !request.To.Any())
                {
                    context.Result = new BadRequestObjectResult("Receiver email addresses are required.");
                    return;
                }
                else
                {
                    foreach (var item in request.To)
                    {
                        if (!ValidationHelper.IsEmail(item))
                        {
                            context.Result = new BadRequestObjectResult($"Receiver email addresses {item} is not.");
                            return;
                        }
                    }
                }

                if (request.Cc is not null && request.Cc.Any())
                {
                    foreach (var item in request.Cc)
                    {
                        if (!ValidationHelper.IsEmail(item))
                        {
                            context.Result = new BadRequestObjectResult($"CC email addresses {item} is not valid.");
                            return;
                        }
                    }
                }

                if (request.Bcc is not null && request.Bcc.Any())
                {
                    foreach (var item in request.Cc)
                    {
                        if (!ValidationHelper.IsEmail(item))
                        {
                            context.Result = new BadRequestObjectResult($"CC email addresses {item} is not valid");
                            return;
                        }
                    }

                }

                if (request.Files is not null && request.Files.Any())
                {
                    foreach (var item in request.Files)
                    {
                        if (!FileValidationHelper.IsFileValid(item, 1024 * 1024, _extensions))
                        {
                            context.Result = new BadRequestObjectResult($"File {item} are not correct.");
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.UrlCallback))
                {

                    if (!ValidationHelper.IsUrl(request.UrlCallback))
                    {
                        context.Result = new BadRequestObjectResult($"Callback url {request.UrlCallback} are not correct.");
                        return;
                    }


                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No additional processing required after action executes.
        }
    }
}
