using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(EscsDbContext context, ILogger<RoleRepository> logger) : base(context, logger)
        {
        }
    }
}
