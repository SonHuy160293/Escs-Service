namespace ESCS.Domain.Dtos
{
    internal class AuthenticationDTO
    {
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}