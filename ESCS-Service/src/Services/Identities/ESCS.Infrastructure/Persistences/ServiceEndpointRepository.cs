using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Infrastructure.Persistences
{
    public class ServiceEndpointRepository : GenericRepository<ServiceEndpoint>, IServiceEndpointRepository
    {
        public ServiceEndpointRepository(EscsDbContext context, ILogger<ServiceEndpointRepository> logger) : base(context, logger)
        {
        }
    }
}
