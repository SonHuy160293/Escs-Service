namespace UserEmailConfiguration.Cache.Models
{
    public class UserEmailConfig
    {
        public long Id { get; set; }
        public string SmtpEmail { get; set; } = default!;
        public string SmtpServer { get; set; } = default!;
        public string SmtpPassword { get; set; } = default!;
        public int SmtpPort { get; set; }
        public long ServiceId { get; set; }
        public long UserId { get; set; }
        public bool IsEnableSsl { get; set; }
    }
}
