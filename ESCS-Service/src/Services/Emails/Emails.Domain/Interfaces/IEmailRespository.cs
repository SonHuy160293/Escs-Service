using Emails.Domain.Models;

namespace Emails.Domain.Interfaces
{
    public interface IEmailRepository
    {
        Task<bool> AddEmailMessageAsync(Email email);
        Task<Email?> GetEmailMessageByIdAsync(Guid id);
    }
}
