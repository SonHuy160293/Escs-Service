using Emails.Domain.Models;
using UserEmailConfiguration.Cache.Models;

namespace Emails.Application.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(UserEmailConfig userEmailConfig, Email email);
    }
}
