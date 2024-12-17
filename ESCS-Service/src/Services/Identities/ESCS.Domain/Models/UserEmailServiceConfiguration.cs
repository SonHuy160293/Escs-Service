using Core.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESCS.Domain.Models
{
    public class UserEmailServiceConfiguration : BaseEntity
    {
        public string SmtpEmail { get; set; } = default!;
        public string SmtpPassword { get; set; } = default!;
        public int SmtpPort { get; set; }


        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public long ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; }
        public bool IsActive { get; set; }
    }
}
