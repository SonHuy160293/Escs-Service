using ESCS.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ESCS.Application.Extensions
{
    public static class TokenExtension
    {
        private static IConfiguration _configuration;

        // Set configuration via a method, as you cannot inject into a static class
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //<summary>
        // generate a jwt token using given user's informations and the configure settings
        // return AuthenticationResponse that includes token
        public static string CreateAccessToken(User user, List<string>? userRoles, bool isSytemToken)
        {
            //create a datetime object representing the token expiration time by adding the number of minutes specified in configuration
            DateTime expiration = isSytemToken ? DateTime.Now.AddDays(100) : DateTime.Now.AddMinutes(30);

            //create an array of Claim objects representing the user's claim, such as their Id, name, email,...
            var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  //JWT unique ID
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
, //Issued at (date and time of token generation)
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),

        };

            if (userRoles is not null)
            {
                userRoles?.ForEach(role =>
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                });
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));

            }


            // create a symmetricSecurityKey object using the key specified in configuration
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // create a signingCreadentials object with the security with the security key and the HMACSHA256 algorithm
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // create a JwtSecurityToken object with the given issuer, audience, claims, expiration and signing credential
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                 claims,
                 expires: expiration,
                 signingCredentials: signingCredentials);


            // create a JwtSecurityTokenHandler object and use it to write a token string
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            // create and return an AuthenticationResponse object contain token, user email, ....
            return token;



        }

        public static ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
                ValidateLifetime = false,
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }


        public static string CreateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
