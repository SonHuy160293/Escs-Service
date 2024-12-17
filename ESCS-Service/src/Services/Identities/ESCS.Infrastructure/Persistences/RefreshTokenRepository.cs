using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(EscsDbContext context, ILogger<GenericRepository<RefreshToken>> logger) : base(context, logger)
        {
        }
    }
}
