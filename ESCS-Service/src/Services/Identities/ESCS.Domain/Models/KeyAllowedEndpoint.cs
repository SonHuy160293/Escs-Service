using Core.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESCS.Domain.Models
{
    public class KeyAllowedEndpoint : BaseEntity
    {
        public bool IsActive { get; set; }

        public long UserApiKeyId { get; set; }
        [ForeignKey(nameof(UserApiKeyId))]
        public virtual UserApiKey UserApiKey { get; set; }

        public long EndpointId { get; set; }
        [ForeignKey(nameof(EndpointId))]
        public virtual ServiceEndpoint ServiceEndpoint { get; set; }
    }
}
