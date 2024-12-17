using static Logs.API.Services.IdentityGrpcService;

namespace Logs.API.Interfaces
{
    public interface IIdentityGrpcService
    {
        public Task<IEnumerable<UserDto>> GetUserInServiceEndpoint(string url, string method);
    }
}
