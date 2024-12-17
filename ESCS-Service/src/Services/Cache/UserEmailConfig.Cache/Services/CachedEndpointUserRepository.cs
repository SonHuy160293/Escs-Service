using Core.Application.Common;
using Core.Application.Services;
using Identity.Cache.Interfaces;
using Identity.Cache.Models;
using Microsoft.Extensions.Logging;
using UserEmailConfiguration.Cache.Services;

namespace Identity.Cache.Services
{
    public class CachedEndpointUserRepository : ICachedEndpointUserRepository
    {

        private readonly ICacheProvider _cacheProvider;
        public const string CacheKey = "EndpointUser_";
        private readonly ILogger<CachedEndpointUserRepository> _logger;

        public CachedEndpointUserRepository(ILogger<CachedEndpointUserRepository> logger, ICacheProvider cacheProvider)
        {
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        public async Task AddEndpointUser(EndpointUser item)
        {
            try
            {
                _logger.LogInformation("{Class} ADDING endpoint user to redis with url:{Url} and method: {Method}", typeof(CachedEndpointUserRepository).Name, item.Url, item.Method);

                await _cacheProvider.SetAsync<EndpointUser>($"{CacheKey}{item.Url}_{item.Method}", item, x =>
                {
                    x.AbsoluteExpiration = 3600;
                    x.SlidingExpiration = 3600;
                });
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogError("{Class} ADDING endpoint user to redis with url:{Url} and method: {Method} catch exception:{Exception}", typeof(CachedEndpointUserRepository).Name, item.Url, item.Method, exception);

                throw;
            }
        }

        public async Task DeleteEndpointUser(string endpoint, string method)
        {
            try
            {
                _logger.LogInformation("{Class} Deleting endpoint user from redis with url:{Url} and method: {Method}", typeof(CachedEndpointUserRepository).Name, endpoint, method);
                await _cacheProvider.RemoveAsync($"{CacheKey}{endpoint}_{method}");
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogError("{Class} DELETING endpoint user to redis with url:{Url} and method: {Method} catch exception:{Exception}", typeof(CachedUserEmailConfigRepository).Name, endpoint, method, exception);

                throw;
            }
        }

        public async Task<IEnumerable<User>> GetEndpointUser(string endpoint, string method)
        {
            try
            {
                _logger.LogInformation("{Class} RETRIVING endpoint user from redis with url:{Url} and method: {Method}", typeof(CachedEndpointUserRepository).Name, endpoint, method);
                var result = await _cacheProvider.GetAsync<EndpointUser>($"{CacheKey}{endpoint}_{method}");
                if (result is not null)
                {
                    return result.Users;
                }
                return null;
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} RETRIVING endpoint to redis  url:{Url} and method: {Method} catch exception:{Exception}", typeof(CachedUserEmailConfigRepository).Name, endpoint, method, exception);

                throw;
            }
        }


    }
}
