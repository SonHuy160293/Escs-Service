using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class UserApiKeyRepository : GenericRepository<UserApiKey>, IUserApiKeyRepository
    {
        public UserApiKeyRepository(EscsDbContext context, ILogger<UserApiKeyRepository> logger) : base(context, logger)
        {
        }
    }
}
