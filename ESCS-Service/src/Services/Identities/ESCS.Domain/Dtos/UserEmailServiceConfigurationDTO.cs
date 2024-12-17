namespace ESCS.Domain.Dtos
{
    internal class UserEmailServiceConfigurationDTO
    {
    }
    public class UserEmailServiceConfigGetDto
    {
        public long? Id { get; set; }
        public string SmtpEmail { get; set; } = default!;

        public int SmtpPort { get; set; }

        public bool IsActive { get; set; }


        public long UserId { get; set; }

        public UserGetDto User { get; set; }

        public long ServiceId { get; set; }

        public ServiceGetDto Service { get; set; }
    }


    public class UserEmailServiceConfigSensitiveGetDto
    {
        public long? Id { get; set; }
        public string SmtpEmail { get; set; } = default!;

        public int SmtpPort { get; set; }
        public string SmtpPassword { get; set; } = default!;

        public long UserId { get; set; }

        public long ServiceId { get; set; }
        public bool IsActive { get; set; }

    }

}
