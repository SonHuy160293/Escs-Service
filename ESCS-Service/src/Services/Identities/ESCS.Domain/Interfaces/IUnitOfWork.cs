using Core.Domain.Interfaces;

namespace ESCS.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserApiKeyRepository UserApiKeyRepository { get; }
        IUserEmailServiceConfigurationRepository UserEmailServiceConfigurationRepository { get; }
        IServiceEndpointRepository ServiceEndpointRepository { get; }
        IKeyAllowedEndpointRepository KeyAllowedEndpointRepository { get; }

        IRefreshTokenRepository RefreshTokenRepository { get; }
    }
}
