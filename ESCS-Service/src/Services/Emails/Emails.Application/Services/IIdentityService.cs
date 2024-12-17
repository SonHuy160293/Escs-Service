using UserEmailConfiguration.Cache.Models;

namespace Emails.Application.Services
{
    public interface IIdentityService
    {
        Task<long> ValidateUserApiKey(string apiKey, string requestPath, string method);
        Task<UserEmailConfig> GetUserEmailConfig(string smtpEmail);
    }
}
