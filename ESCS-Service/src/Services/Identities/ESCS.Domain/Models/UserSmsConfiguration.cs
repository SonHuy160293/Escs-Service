using Core.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESCS.Domain.Models
{
    public class UserSmsConfiguration : BaseEntity
    {
        public string BrandName { get; set; }

        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public long ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; }
        public bool IsActive { get; set; }
    }
}
