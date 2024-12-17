using Core.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESCS.Domain.Models
{

    public class UserApiKey : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public long ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public bool IsActive { get; set; }

    }

}
