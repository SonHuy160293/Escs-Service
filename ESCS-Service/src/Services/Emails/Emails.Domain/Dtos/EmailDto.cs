using Microsoft.AspNetCore.Http;

namespace Emails.Domain.Dtos
{
    public class EmailDto
    {
        public string? From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string? Subject { get; set; }
        public string? BodyText { get; set; }
        public string? BodyHtml { get; set; }
        public bool IsNoReply { get; set; }
        public List<IFormFile>? Files { get; set; } = default!;
        public string ObjectId { get; set; } = default!;
        public string? UrlCallback { get; set; }
        public string? AuthenticationType { get; set; }
        public string? Key { get; set; }
    }
}
