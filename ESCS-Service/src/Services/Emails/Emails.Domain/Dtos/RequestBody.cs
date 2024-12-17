namespace Emails.Domain.Dtos
{
    public class CalbackRequestBody
    {
        public string ObjectId { get; set; } = default!;
        public string Message { get; set; } = default!;
        public bool IsSent { get; set; }
    }

    public class ValidateKeyRequestBody
    {
        public string Key { get; set; }
        public string RequestPath { get; set; }
        public string Method { get; set; }
        public string SmtpEmail { get; set; }
    }


}
