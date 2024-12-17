using Microsoft.AspNetCore.Http;

namespace Core.Application.Helpers
{
    public static class CorrelationIdProvider
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetCorrelationId()
        {
            return _httpContextAccessor?.HttpContext?.Request.Headers["CorrelationId"].FirstOrDefault()
                   ?? Guid.NewGuid().ToString();
        }
    }
}
