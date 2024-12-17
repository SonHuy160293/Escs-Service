using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(EscsDbContext context, ILogger<UserRepository> logger) : base(context, logger)
        {
        }
    }
}
