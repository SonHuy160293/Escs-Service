using Core.Domain.Base;

namespace ESCS.Domain.Models
{
    public class Service : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? BaseUrl = default!;
    }
}
