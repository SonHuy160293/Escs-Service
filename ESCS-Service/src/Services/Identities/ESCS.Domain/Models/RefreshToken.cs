using Core.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESCS.Domain.Models
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool IsRevoked { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = default!;
    }


}

