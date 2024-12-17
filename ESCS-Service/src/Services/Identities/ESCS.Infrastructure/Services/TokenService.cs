using Core.Application.Exceptions;
using ESCS.Application.Extensions;
using ESCS.Application.Services;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace ESCS.Infrastructure.Services
{
    public class TokenService : ITokenService
    {

        // Dependency on the database context used to interact with the database.
        private readonly IUnitOfWork _unitOfWork;
        private static IConfiguration _configuration;
        // Constructor that initializes the TokenService with an instance of the database context.
        public TokenService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        // Asynchronously saves a new refresh token to the database.
        public async Task<RefreshToken> SaveRefreshToken(long userId)
        {
            // Create a new RefreshToken object.
            var refreshToken = new RefreshToken
            {
                UserId = userId,  // Set the username associated with the token.
                Token = TokenExtension.CreateRefreshToken(),  // Set the token value.
                ExpiredDate = DateTime.UtcNow.AddDays(7)  // Set the expiration date to 7 days from the current UTC date/time.
            };

            await _unitOfWork.RefreshTokenRepository.Add(refreshToken);

            await _unitOfWork.SaveChangesAsync();

            return refreshToken;
        }

        // Asynchronously retrieves the username associated with a specific refresh token.
        public async Task<long> RetrieveUserIdByRefreshToken(string refreshToken)
        {

            var tokenRecord = await _unitOfWork.RefreshTokenRepository.FindEntityByQuery(
                t => t.Token == refreshToken && t.ExpiredDate > DateTime.UtcNow);

            // Return the username if the token is found and valid, otherwise null.
            return tokenRecord.UserId;
        }

        // Asynchronously revokes (deletes) a refresh token from the database.
        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            // Asynchronously find the refresh token in the database.
            var tokenRecord = await _unitOfWork.RefreshTokenRepository.FindEntityByQuery(
                t => t.Token == refreshToken) ?? throw new NotFoundException("Token not found");

            // If the token is found, remove it from the DbSet.
            if (tokenRecord != null)
            {
                tokenRecord.IsRevoked = true;
                _unitOfWork.RefreshTokenRepository.Update(tokenRecord);
                // Save changes to the database asynchronously to reflect the token removal.
                await _unitOfWork.SaveChangesAsync();
                return true;  // Return true to indicate successful revocation.
            }

            // Return false if no matching token was found, indicating no revocation was performed.
            return false;
        }
    }
}
