using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class KeyAllowedEndpointRepository : GenericRepository<KeyAllowedEndpoint>, IKeyAllowedEndpointRepository
    {
        public KeyAllowedEndpointRepository(EscsDbContext context, ILogger<KeyAllowedEndpointRepository> logger) : base(context, logger)
        {
        }
    }
}
