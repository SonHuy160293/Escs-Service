namespace Core.Domain.Base
{
    public abstract class BaseEvent
    {
        public string CorrelationId { get; set; } = default!;
    }
}
