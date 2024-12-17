using MassTransit;

namespace Core.Infrastructure.Services
{
    public abstract class BaseConsumer<T> : IConsumer<T> where T : class
    {
        public async Task Consume(ConsumeContext<T> context)
        {
            // Retrieve and log the X-Correlation-ID header if it exists
            if (context.Headers.TryGetHeader("CorrelationId", out var correlationId))
            {
                Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId);
            }
            else
            {
                // Generate a new Correlation ID if not provided
                correlationId = Guid.NewGuid().ToString();
                Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId);
            }

            // Call the abstract HandleConsume method for derived consumers
            await HandleConsume(context);
        }

        // Abstract method to be implemented by derived classes
        protected abstract Task HandleConsume(ConsumeContext<T> context);
    }
}
