using Core.Infrastructure.Persistences;
using ESCS.Domain.Interfaces;

namespace ESCS.Infrastructure.Persistences
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {

        private readonly IUserRepository _userRepository;
        private readonly IUserApiKeyRepository _userApiKeyRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserEmailServiceConfigurationRepository _userEmailServiceConfigurationRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceEndpointRepository _serviceEndpointRepository;
        private readonly IKeyAllowedEndpointRepository _keyAllowedEndpointRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UnitOfWork(EscsDbContext context, IServiceProvider provider) : base(context, provider)
        {
            _userRepository = (IUserRepository)provider.GetService(typeof(IUserRepository));
            _userEmailServiceConfigurationRepository = (IUserEmailServiceConfigurationRepository)provider.GetService(typeof(IUserEmailServiceConfigurationRepository));
            _roleRepository = (IRoleRepository)provider.GetService(typeof(IRoleRepository));
            _serviceRepository = (IServiceRepository)provider.GetService(typeof(IServiceRepository));
            _userApiKeyRepository = (IUserApiKeyRepository)provider.GetService(typeof(IUserApiKeyRepository));
            _serviceEndpointRepository = (IServiceEndpointRepository)provider.GetService(typeof(IServiceEndpointRepository));
            _keyAllowedEndpointRepository = (IKeyAllowedEndpointRepository)provider.GetService(typeof(IKeyAllowedEndpointRepository));
            _refreshTokenRepository = (IRefreshTokenRepository)provider.GetService(typeof(IRefreshTokenRepository));
        }

        public IUserRepository UserRepository => _userRepository;

        public IServiceRepository ServiceRepository => _serviceRepository;

        public IRoleRepository RoleRepository => _roleRepository;

        public IUserApiKeyRepository UserApiKeyRepository => _userApiKeyRepository;

        public IUserEmailServiceConfigurationRepository UserEmailServiceConfigurationRepository => _userEmailServiceConfigurationRepository;

        public IServiceEndpointRepository ServiceEndpointRepository => _serviceEndpointRepository;

        public IKeyAllowedEndpointRepository KeyAllowedEndpointRepository => _keyAllowedEndpointRepository;

        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository;
    }
}
