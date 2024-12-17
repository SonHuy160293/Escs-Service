using Core.Application.Common;
using Core.Application.Services;
using Microsoft.Extensions.Logging;
using UserEmailConfiguration.Cache.Interfaces;
using UserEmailConfiguration.Cache.Models;

namespace UserEmailConfiguration.Cache.Services
{
    public class CachedUserEmailConfigRepository : ICachedUserEmailConfigRepository
    {

        private readonly ICacheProvider _cacheProvider;
        public const string CacheKey = "UserEmailConfig_";
        private readonly ILogger<CachedUserEmailConfigRepository> _logger;


        public CachedUserEmailConfigRepository(ICacheProvider cacheProvider, ILogger<CachedUserEmailConfigRepository> logger)
        {
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        //add user email config to redis
        //key is follow template: "Email_" + objectId
        public async Task AddUserEmailConfig(UserEmailConfig item)
        {
            try
            {
                _logger.LogInformation("{Class} ADDING user email config to redis with id:{Id}", typeof(CachedUserEmailConfigRepository).Name, item.Id);

                await _cacheProvider.SetAsync<UserEmailConfig>($"{CacheKey}{item.SmtpEmail}", item, x =>
                {
                    x.AbsoluteExpiration = 3600;
                    x.SlidingExpiration = 3600;
                });
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogError("{Class} ADDING user email config to redis with id:{Id} catch exception:{Exception}", typeof(CachedUserEmailConfigRepository).Name, item.Id, exception);

                throw;
            }
        }

        //delete user email config from redis
        //key is follow template: "Email_" + objectId
        public async Task DeleteUserEmailConfig(string smtpEmail)
        {
            try
            {
                _logger.LogInformation("{Class} DELETING user email config from redis with Id:{Id}", typeof(CachedUserEmailConfigRepository).Name, smtpEmail);
                await _cacheProvider.RemoveAsync($"{CacheKey}{smtpEmail}");
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogError("{Class} DELETING user email config to redis with objectId:{ObjectId} catch exception:{Exception}", typeof(CachedUserEmailConfigRepository).Name, smtpEmail, exception);

                throw;
            }
        }

        //retrive user email config from redis
        //key is follow template: "Email_" + objectId
        public async Task<UserEmailConfig> GetUserEmailConfig(string smtpEmail)
        {
            try
            {
                _logger.LogInformation("{Class} RETRIVING user email config from redis with Id:{Id}", typeof(CachedUserEmailConfigRepository).Name, smtpEmail);
                var result = await _cacheProvider.GetAsync<UserEmailConfig>($"{CacheKey}{smtpEmail}");
                return result;
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} RETRIVING user email config to redis with Id:{Id} catch exception:{Exception}", typeof(CachedUserEmailConfigRepository).Name, smtpEmail, exception);

                throw;
            }

        }


    }
}
