using Microsoft.Extensions.Logging;
using Polly;
using System.Net;

namespace Core.Application.Extensions
{
    public static class RetryExtension
    {
        /// <summary>
        /// Creates a retry policy that will attempt to retry an HTTP request a specified number of times
        /// in case of transient failures such as HttpRequestException or TaskCanceledException.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="delay">The delay between each retry attempt.</param>
        /// <param name="logger">The logger used to log retry information.</param>
        /// <returns>An asynchronous policy that retries the operation on failure.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, TimeSpan delay, ILogger logger)
        {
            return Policy
                // Handle HttpRequestException and TaskCanceledException as transient errors that may be retried.
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                // Specify the retry behavior
                .OrResult<HttpResponseMessage>(response =>
            (int)response.StatusCode >= 500 && (int)response.StatusCode < 600) // Retry on 5xx status codes
                .WaitAndRetryAsync(retryCount,
                    // Delay between retries - can be customized per attempt if desired.
                    attempt => delay,
                    // Action to perform on each retry attempt
                    (outcome, timeSpan, attempt, context) =>
                    {
                        // Log a warning message indicating which retry attempt has failed and the reason
                        logger.LogWarning($"[Retry {attempt} encountered an error: {outcome.Exception?.Message.ToString() ?? string.Empty}. Waiting {timeSpan.TotalSeconds} seconds before next retry.");
                    });
        }

        /// <summary>
        /// Creates a timeout policy that limits the duration of an HTTP request.
        /// If the request does not complete within the specified timeout duration,
        /// it will be canceled.
        /// </summary>
        /// <param name="timeout">The maximum time allowed for the operation to complete.</param>
        /// <param name="logger">The logger used to log timeout information.</param>
        /// <returns>An asynchronous policy that enforces a timeout on the operation.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout, ILogger logger)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(timeout, Polly.Timeout.TimeoutStrategy.Pessimistic, onTimeoutAsync: (context, timeSpan, task) =>
            {
                logger.LogWarning("Request timed out after {TimeoutSeconds} seconds.", timeSpan.TotalSeconds);
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Creates a circuit breaker policy that prevents further attempts to make requests after a certain 
        /// number of failures within a specified timeframe.
        /// </summary>
        /// <param name="failureThreshold">The number of failed requests that will trigger the circuit breaker.</param>
        /// <param name="durationOfBreak">The duration for which the circuit remains open after being triggered.</param>
        /// <param name="logger">The logger used to log circuit breaker information.</param>
        /// <returns>An asynchronous circuit breaker policy that controls the execution of requests.</returns>
        public static IAsyncPolicy GetCircuitBreakerPolicy(int failureThreshold, TimeSpan durationOfBreak, ILogger logger)
        {
            return Policy
                 // Handle HttpRequestException as the failure that will trigger the circuit breaker.
                 .Handle<HttpRequestException>()
                 // Define the circuit breaker behavior
                 .CircuitBreakerAsync(
                     failureThreshold, // Number of failures to trigger the circuit breaker
                     durationOfBreak,  // Duration for which the circuit is open
                                       // Action to execute when the circuit is broken
                     onBreak: (exception, duration) =>
                     {
                         // Log a warning indicating the circuit has been broken and the reason
                         logger.LogWarning($"Circuit broken for {duration.TotalSeconds} seconds due to: {exception.Message ?? string.Empty}");
                     },
                     // Action to execute when the circuit is reset
                     onReset: () => logger.LogInformation("Circuit reset!"),
                     // Action to execute when the circuit is in half-open state, allowing a test request
                     onHalfOpen: () => logger.LogInformation("Circuit in test mode."));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy(ILogger logger)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode) // Trigger fallback for unsuccessful responses
                .FallbackAsync(
                    fallbackAction: (responseToFailedRequest, context, cancellationToken) => FallbackAction(responseToFailedRequest, context, cancellationToken, logger),
                    onFallbackAsync: (response, context) => OnFallbackAsync(response, context, logger)
                );
        }

        private static Task OnFallbackAsync(DelegateResult<HttpResponseMessage> response, Context context, ILogger logger)
        {
            logger.LogError("Fallback triggered due to: {Error}",
                response.Exception?.Message ?? $"HTTP {response.Result?.StatusCode}");
            return Task.CompletedTask;
        }

        private static Task<HttpResponseMessage> FallbackAction(DelegateResult<HttpResponseMessage> responseToFailedRequest, Context context, CancellationToken cancellationToken, ILogger logger)
        {
            logger.LogWarning("Fallback executed. Returning default response.");

            var fallbackResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) // Default fallback status
            {
                Content = new StringContent($"The fallback executed. The original error was: {responseToFailedRequest.Exception?.Message ?? responseToFailedRequest.Result?.ReasonPhrase}.")
            };

            return Task.FromResult(fallbackResponse);
        }

    }
}
