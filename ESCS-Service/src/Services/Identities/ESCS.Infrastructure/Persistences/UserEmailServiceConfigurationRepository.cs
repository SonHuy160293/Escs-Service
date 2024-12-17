using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class UserEmailServiceConfigurationRepository : GenericRepository<UserEmailServiceConfiguration>, IUserEmailServiceConfigurationRepository
    {
        public UserEmailServiceConfigurationRepository(EscsDbContext context, ILogger<UserEmailServiceConfigurationRepository> logger) : base(context, logger)
        {
        }
    }
}
