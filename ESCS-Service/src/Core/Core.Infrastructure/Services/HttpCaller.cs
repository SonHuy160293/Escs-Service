using Core.Application.Common;
using Core.Application.Extensions;
using Core.Application.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.Timeout;

namespace Core.Infrastructure.Services
{
    public class HttpCaller : IHttpCaller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpCaller> _logger;

        public HttpCaller(IHttpClientFactory httpClientFactory, ILogger<HttpCaller> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }


        //send get request
        public async Task<BaseHttpResult<T>> GetAsync<T>(HttpCallOptions httpCallOptions) where T : class
        {

            //create HttpRequestMessage with method Get
            var requestMessage = HttpContextExtension.CreateHttpRequestMessage(HttpMethod.Get, httpCallOptions);

            //send request
            var baseHttpResult = await SendHttpRequestMessageAsync<T>(requestMessage, httpCallOptions.IsSerializedObject, httpCallOptions.IsRetry);

            return baseHttpResult;

        }


        //send post request
        public async Task<BaseHttpResult<TResponse>> PostAsync<TRequest, TResponse>(HttpCallOptions<TRequest> httpCallOptions)
           where TRequest : class

        {
            //create HttpRequestMessage with method Post
            var requestMessage = HttpContextExtension.CreateHttpRequestMessage(HttpMethod.Post, httpCallOptions);

            //send request
            var baseHttpResult = await SendHttpRequestMessageAsync<TResponse>(requestMessage, httpCallOptions.IsSerializedObject, httpCallOptions.IsRetry);

            return baseHttpResult;
        }



        private async Task<BaseHttpResult<T>> SendHttpRequestMessageAsync<T>(HttpRequestMessage httpRequestMessage, bool isSerializedObject, bool isRetry)
        {
            var baseHttpResult = new BaseHttpResult<T>();
            var response = new HttpResponseMessage();
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                if (isRetry) // Send request with retry policy
                {


                    // Define policies
                    var timeoutPolicy = RetryExtension.GetTimeoutPolicy(TimeSpan.FromSeconds(10), _logger); // Set your desired timeout duration here
                    var retryPolicy = RetryExtension.GetRetryPolicy(3, TimeSpan.FromSeconds(5), _logger);
                    var circuitBreakerPolicy = RetryExtension.GetCircuitBreakerPolicy(2, TimeSpan.FromSeconds(30), _logger);
                    var fallbackPolicy = RetryExtension.GetFallbackPolicy(_logger);
                    // Combine policies using WrapAsync
                    // Use timeout policy first
                    response = await timeoutPolicy.ExecuteAsync(async () =>
                    {
                        return await retryPolicy.ExecuteAsync(async () =>
                        {
                            return await circuitBreakerPolicy.ExecuteAsync(async () =>
                            {
                                return await fallbackPolicy.ExecuteAsync(async () =>
                                {
                                    // Execute the HTTP request
                                    return await httpClient.SendAsync(HttpContextExtension.CloneHttpRequestMessage(httpRequestMessage));
                                });
                            });
                        });
                    });


                }
                else // Send request without retry policy
                {
                    response = await httpClient.SendAsync(httpRequestMessage);

                }

                baseHttpResult.StatusCode = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    if (isSerializedObject)
                    {
                        var responseStream = await response.Content.ReadAsStringAsync();
                        baseHttpResult.Data = JsonConvert.DeserializeObject<T>(responseStream);
                    }
                    baseHttpResult.Succeeded = true;
                }
                else
                {
                    baseHttpResult.Succeeded = false;
                }
            }
            catch (TimeoutRejectedException)
            {
                baseHttpResult.Succeeded = false;
                _logger.LogWarning("The HTTP request timed out.");
                throw;
            }
            catch (HttpRequestException)
            {
                baseHttpResult.Succeeded = false;
                _logger.LogError("The HTTP request failed.");
                throw;
            }
            catch (Exception ex)
            {
                baseHttpResult.Succeeded = false;
                _logger.LogError(ex, "An error occurred while sending the HTTP request.");
                throw;
            }

            return baseHttpResult;
        }






    }
}
