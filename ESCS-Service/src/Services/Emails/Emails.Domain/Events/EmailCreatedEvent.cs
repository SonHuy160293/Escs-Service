using Core.Domain.Base;

namespace Emails.Domain.Events
{
    public class EmailCreatedEvent : BaseEvent
    {
        public Guid EmailId { get; set; }
        public string ObjectId { get; set; }
    }
}
