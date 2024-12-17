using Core.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESCS.Domain.Models
{
    public class ServiceEndpoint : BaseEntity
    {
        public string Method { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; }
        public long ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; }
    }
}
