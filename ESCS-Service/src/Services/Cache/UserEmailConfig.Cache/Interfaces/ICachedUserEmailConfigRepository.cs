using UserEmailConfiguration.Cache.Models;

namespace UserEmailConfiguration.Cache.Interfaces
{
    public interface ICachedUserEmailConfigRepository
    {
        Task AddUserEmailConfig(UserEmailConfig item);
        Task DeleteUserEmailConfig(string smtpEmail);
        Task<UserEmailConfig> GetUserEmailConfig(string smtpEmail);
    }
}
