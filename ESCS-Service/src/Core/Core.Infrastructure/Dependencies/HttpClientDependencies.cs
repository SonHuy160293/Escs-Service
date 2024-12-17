using Core.Application.Services;
using Core.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Core.Infrastructure.Dependencies
{
    public static class HttpClientDependencies
    {

        public static IServiceCollection AddCustomHttpClient(this IServiceCollection services)
        {
            //// Add HttpClient with Polly policies
            //services.AddHttpClient("retry-client")
            //    .AddPolicyHandler(GetRetryPolicy(3, TimeSpan.FromSeconds(2)))
            //    .AddPolicyHandler(GetCircuitBreakerPolicy(2, TimeSpan.FromSeconds(30)));

            services.AddHttpClient();

            services.AddTransient<IHttpCaller, HttpCaller>();

            return services;
        }


        // Define Polly Policies
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, TimeSpan delay)
        {
            return (IAsyncPolicy<HttpResponseMessage>)Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(retryCount,
                    attempt => delay,
                    (outcome, timeSpan, attempt, context) =>
                    {
                        Console.WriteLine($"Retry {attempt} encountered an error: {outcome.InnerException?.ToString()}. Waiting {timeSpan.TotalSeconds} seconds before next retry.");
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int failureThreshold, TimeSpan durationOfBreak)
        {
            return (IAsyncPolicy<HttpResponseMessage>)Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    failureThreshold,
                    durationOfBreak,
                    onBreak: (exception, duration) =>
                    {
                        Console.WriteLine($"Circuit broken for {duration.TotalSeconds} seconds due to: {exception.Message}");
                    },
                    onReset: () => Console.WriteLine("Circuit reset!"),
                    onHalfOpen: () => Console.WriteLine("Circuit in test mode."));
        }

    }
}
