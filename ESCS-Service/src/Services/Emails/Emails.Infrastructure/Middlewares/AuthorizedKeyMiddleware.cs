using Emails.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Emails.Infrastructure.Middlewares
{
    public class AuthorizedKeyMiddleware
    {
        private RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;
        private readonly IIdentityGrpcService _identityGrpcService;
        public AuthorizedKeyMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor,
            IIdentityService identityService, IIdentityGrpcService identityGrpcService)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _identityGrpcService = identityGrpcService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized: API key is missing.");
                return;
            }

            //var userId = await _identityService.ValidateUserApiKey(extractedApiKey, context.Request.Path, context.Request.Method);

            try
            {
                var userId = await _identityGrpcService.ValidateUserApiKey(extractedApiKey, context.Request.Path, context.Request.Method);

                context.Items["UserId"] = userId;

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized: API key is not valid.");
                return;
            }
        }
    }
}
