using ESCS.Domain.Models;

namespace ESCS.Application.Services
{
    public interface ITokenService
    {
        Task<RefreshToken> SaveRefreshToken(long userId);
        Task<long> RetrieveUserIdByRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken);
    }
}
