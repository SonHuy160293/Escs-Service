using Identity.Cache.Models;

namespace Identity.Cache.Interfaces
{
    public interface ICachedEndpointUserRepository
    {
        Task AddEndpointUser(EndpointUser item);
        Task DeleteEndpointUser(string endpoint, string method);
        Task<IEnumerable<User>> GetEndpointUser(string endpoint, string method);
    }
}
