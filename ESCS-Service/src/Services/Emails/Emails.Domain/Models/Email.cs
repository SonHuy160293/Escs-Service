using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Emails.Domain.Models
{
    public class Email
    {
        // Thông tin người gửi và người nhận
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string ObjectId { get; set; } = default!;
        public string? UrlCallback { get; set; }
        public string? AuthenticationType { get; set; }
        public string? Key { get; set; }

        public string From { get; set; } = default!; // Địa chỉ email của người gửi
        public List<string> To { get; set; } = new List<string>(); // Danh sách người nhận
        public List<string> Cc { get; set; } = new List<string>(); // Danh sách người nhận bản sao
        public List<string> Bcc { get; set; } = new List<string>(); // Danh sách người nhận bản sao ẩn

        public string Subject { get; set; } = default!; // Tiêu đề email
        public string BodyHtml { get; set; } = default!; // Nội dung HTML

        public DateTime? Date { get; set; } = default!;  // Ngày gửi email

        public List<string>? Attachments = default!;
        public bool IsNoReply { get; set; }
        public int Status { get; set; }



    }
}
