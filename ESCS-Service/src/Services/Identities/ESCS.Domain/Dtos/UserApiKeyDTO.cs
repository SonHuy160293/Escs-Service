namespace ESCS.Domain.Dtos
{
    internal class UserApiKeyDTO
    {
    }

    public class UserApiKeyGetDto
    {
        public string Key { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; private set; }

    }

    public class UserApiKeyDetailDto
    {
        public long Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; private set; }

        public List<KeyAllowedEndpointDetailDto> AllowedEndpoints { get; set; }
    }
}
