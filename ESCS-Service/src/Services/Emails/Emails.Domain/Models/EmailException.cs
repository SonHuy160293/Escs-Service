using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Emails.Domain.Models
{
    public class EmailException
    {
        [BsonRepresentation(BsonType.String)]
        public Guid FaultId { get; set; }
        public List<ExceptionInfoDocument> Exceptions { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? FaultedMessageId { get; set; }
        public object Host { get; set; } // Make sure this type matches your Host details.
        [BsonRepresentation(BsonType.String)]
        public Guid MessageId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ExceptionInfoDocument
    {
        public string ExceptionType { get; set; }
        public ExceptionInfoDocument? InnerException { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }
}
