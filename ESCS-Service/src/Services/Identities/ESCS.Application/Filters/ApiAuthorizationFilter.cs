using ESCS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESCS.Application.Filters
{
    public class ApiKeyAuthorizationFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApiKeyAuthorizationFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if the request contains the X-Api-Key header
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Validate the API key
            var apiKey = await _unitOfWork.UserApiKeyRepository.FindEntityByQuery(k => k.Key == extractedApiKey.ToString());

            if (apiKey is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = await _unitOfWork.UserRepository.GetById(apiKey.UserId);

            if (user.RoleId != 21 || user is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
