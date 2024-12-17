using Core.Application.Common;
using Core.Application.Services;
using Emails.Application.Services;
using Emails.Domain.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;
using UserEmailConfiguration.Cache.Models;

namespace Emails.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly IHttpCaller _httpCaller;
        private readonly ILogger<IdentityService> _logger;
        private readonly IConfiguration _configuration;

        public IdentityService(IHttpCaller httpCaller, ILogger<IdentityService> logger, IConfiguration configuration)
        {
            _httpCaller = httpCaller;
            _logger = logger;
            _configuration = configuration;
        }


        public async Task<UserEmailConfig> GetUserEmailConfig(string smtpEmail)
        {

            var key = _configuration["ApiKey:Identity"];

            var queryString = new List<string>()
            {
                $"email={smtpEmail}"
            };



            // Declare HttpCallOptions: isDeserialized, isRetry, requestBody, callBackUrl, authenticationType, key
            var httpCallOptions = HttpCallOptions
                .Authenticated(isSerialized: true, isRetry: false, "http://localhost:5212/api/Users/email-config/detail", queryString, "X-Api-Key", key);


            // Send request to callback URL
            var response = await _httpCaller.GetAsync<UserEmailConfig>(httpCallOptions);

            // Check if response is successful and return the data
            if (response == null || !response.Succeeded || response.Data == default)
            {
                throw new AuthenticationException("Email not valid"); // Handle the error accordingly
            }

            return response.Data;
        }

        public async Task<long> ValidateUserApiKey(string apiKey, string requestPath, string method)
        {
            // Declare request body
            var requestBody = new ValidateKeyRequestBody()
            {
                Key = apiKey,
                Method = method,
                RequestPath = requestPath,
            };

            // Declare HttpCallOptions: isDeserialized, isRetry, requestBody, callBackUrl, authenticationType, key
            var httpCallOptions = HttpCallOptions<ValidateKeyRequestBody>
                .UnAuthenticated(isSerialized: true, isRetry: false, requestBody, "http://localhost:5212/api/Users/validate-key", null);

            // Send request to callback URL
            var response = await _httpCaller.PostAsync<ValidateKeyRequestBody, long>(httpCallOptions);

            // Check if response is successful and return the data
            if (response == null || !response.Succeeded || response.Data == default)
            {
                throw new AuthenticationException("Failed to validate API key."); // Handle the error accordingly
            }

            return response.Data;
        }



    }
}
