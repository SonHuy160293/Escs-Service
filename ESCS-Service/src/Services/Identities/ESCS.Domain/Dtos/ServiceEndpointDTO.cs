namespace ESCS.Domain.Dtos
{
    internal class ServiceEndpointDTO
    {
    }

    public class ServiceEndpointGetDto
    {
        public long Id { get; set; }
        public string Method { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; }


    }
}
