using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace Core.Infrastructure.Middlewares
{
    public class HttpContextLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpContextLoggingMiddleware> _logger;

        public HttpContextLoggingMiddleware(RequestDelegate next, ILogger<HttpContextLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var userId = context.Items["UserId"] ?? string.Empty;

            // Log Request
            context.Request.EnableBuffering();  // Allows reading the request body multiple times
            var requestBody = await GetRequestBodyAsync(context.Request);
            _logger.LogInformation("INCOMING {Method} {LogType} from userId: {UserId}: with body: {Body}", "Request", context.Request.Method, userId, requestBody);

            await _next(context);

            // Log Response
            stopwatch.Stop();
            var responseBody = await GetResponseBodyAsync(context.Response);
            _logger.LogInformation("OUTGOING {Method} {LogType} to userId: {UserId} with status: {StatusCode} and response: {Body} | Processed in {ElapsedMilliseconds} ms",
                context.Request.Method, "Response", userId, context.Response.StatusCode, responseBody, stopwatch.ElapsedMilliseconds);
        }

        private async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            if (request.HasFormContentType)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Form Fields:");
                foreach (var field in request.Form.Where(kvp => kvp.Value.Any()))
                {
                    sb.AppendLine($"{field.Key}: {field.Value}");
                }

                if (request.Form.Files.Any())
                {
                    sb.AppendLine("Files:");
                    foreach (var file in request.Form.Files)
                    {
                        sb.AppendLine($"{file.Name}: {file.FileName}");
                    }
                }

                return sb.ToString();
            }

            return await ReadStreamAsync(request.Body);
        }

        private async Task<string> GetResponseBodyAsync(HttpResponse response)
        {
            if (response.Body.CanSeek)
            {
                return await ReadStreamAsync(response.Body);
            }
            return string.Empty;
        }

        private async Task<string> ReadStreamAsync(Stream stream)
        {
            stream.Position = 0;  // Reset stream position for reading
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            stream.Position = 0;  // Reset stream position after reading
            return content;
        }
    }
}
