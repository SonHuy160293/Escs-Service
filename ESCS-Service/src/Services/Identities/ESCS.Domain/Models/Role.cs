using Core.Domain.Base;

namespace ESCS.Domain.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = default!;
    }
}
