namespace ESCS.Domain.Dtos
{
    internal class ServiceDTO
    {
    }

    public class ServiceGetDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? BaseUrl = default!;
    }
}
