using ESCS.Domain.Models;

namespace ESCS.Domain.Dtos
{
    internal class KeyAllowedEndpointDTO
    {
    }

    public class KeyAllowedEndpointGetDto
    {
        public bool IsActive { get; set; }


        public virtual UserApiKeyGetDto UserApiKey { get; set; }


        public virtual ServiceEndpoint ServiceEndpoint { get; set; }
    }

    public class KeyAllowedEndpointDetailDto
    {
        public bool IsActive { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }

    public class ServiceEndpointRegisterByUserGetDto
    {
        public long ServiceId { get; set; }
        public long UserId { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
    }

    public class UsersRegisterInEndpointGetDto
    {
        public long UserId { get; set; }
        public string Email { get; set; }
    }
}