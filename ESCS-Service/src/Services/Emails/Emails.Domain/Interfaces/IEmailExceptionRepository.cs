using Emails.Domain.Models;

namespace Emails.Domain.Interfaces
{
    public interface IEmailExceptionRepository
    {
        Task<bool> AddEmailExceptionAsync(EmailException emailException);
        Task<EmailException> GetEmailExceptionByIdAsync(Guid id);
    }
}
