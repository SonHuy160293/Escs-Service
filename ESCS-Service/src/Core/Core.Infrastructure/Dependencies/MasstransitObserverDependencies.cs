using Core.Domain.Base;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Dependencies
{
    internal class MasstransitObserverDependencies
    {
    }

    public class CorrelationIdPublishObserver : IPublishObserver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdPublishObserver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            string correlationId = "";
            if (context.Message is BaseEvent baseEvent)
            {
                // Now you can access the fields of the EmailSentEvent message
                correlationId = baseEvent.CorrelationId;

            }

            context.Headers.Set("CorrelationId", correlationId);
            Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId);

            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class => Task.CompletedTask;
        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class => Task.CompletedTask;
    }

    public class CorrelationIdSendObserver : ISendObserver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdSendObserver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task PreSend<T>(SendContext<T> context) where T : class
        {

            string correlationId = "";


            if (context.Message is BaseEvent baseEvent)
            {
                // Now you can access the fields of the EmailSentEvent message
                correlationId = baseEvent.CorrelationId;

            }

            context.Headers.Set("CorrelationId", correlationId);
            Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId);

            return Task.CompletedTask;
        }

        public Task PostSend<T>(SendContext<T> context) where T : class => Task.CompletedTask;
        public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class => Task.CompletedTask;
    }


    public class CorrelationIdMiddleware<T> : IFilter<ConsumeContext<T>> where T : class
    {
        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            // Try to retrieve CorrelationId from headers
            if (context.Headers.TryGetHeader("X-Correlation-ID", out var correlationId))
            {
                Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId); // Push to LogContext for this message's lifecycle
            }
            //else
            //{
            //    var newCorrelationId = Guid.NewGuid().ToString();
            //    context.Headers. = newCorrelationId;
            //    Serilog.Context.LogContext.PushProperty("CorrelationId", newCorrelationId);  // Set new CorrelationId if missing
            //}

            return next.Send(context); // Pass to next pipe in pipeline
        }

        public void Probe(ProbeContext context) { context.CreateFilterScope("CorrelationIdMiddleware"); }
    }
}
