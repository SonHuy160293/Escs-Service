using Core.Application.Helpers;
using Emails.Application.Features.Emails;
using Emails.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserEmailConfiguration.Cache.Interfaces;

namespace Emails.API.Filters
{
    public class ValidateSendMailRequestFilter : IAsyncActionFilter
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


        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No additional processing required after action executes.
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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

                // Validate 'From' email
                if (!ValidationHelper.IsEmail(request.From))
                {
                    context.Result = new BadRequestObjectResult("Sender email address is invalid.");
                    return;
                }

                // Check user email config
                var userEmailConfig = await _cachedUserEmailConfigRepository.GetUserEmailConfig(request.From);
                if (userEmailConfig is null)
                {
                    userEmailConfig = await _identityGrpcService.GetUserEmailConfig(request.From);
                    await _cachedUserEmailConfigRepository.AddUserEmailConfig(userEmailConfig);
                }

                // Validate 'To' email addresses
                if (request.To is null || !request.To.Any())
                {
                    context.Result = new BadRequestObjectResult("Receiver email addresses are required.");
                    return;
                }

                foreach (var item in request.To)
                {
                    if (!ValidationHelper.IsEmail(item))
                    {
                        context.Result = new BadRequestObjectResult($"Receiver email address {item} is invalid.");
                        return;
                    }
                }

                // Validate CC emails
                if (request.Cc is not null)
                {
                    foreach (var item in request.Cc)
                    {
                        if (!ValidationHelper.IsEmail(item))
                        {
                            context.Result = new BadRequestObjectResult($"CC email address {item} is invalid.");
                            return;
                        }
                    }
                }

                // Validate files
                if (request.Files is not null && request.Files.Any())
                {
                    foreach (var file in request.Files)
                    {
                        if (!FileValidationHelper.IsFileValid(file, 1024 * 1024, _extensions))
                        {
                            context.Result = new BadRequestObjectResult($"File {file.FileName} is not valid.");
                            return;
                        }
                    }
                }

                // Validate callback URL
                if (!string.IsNullOrEmpty(request.UrlCallback) && !ValidationHelper.IsUrl(request.UrlCallback))
                {
                    context.Result = new BadRequestObjectResult($"Callback URL {request.UrlCallback} is invalid.");
                    return;
                }
            }

            // Proceed to the next delegate/middleware in the pipeline
            await next();
        }
    }
}
