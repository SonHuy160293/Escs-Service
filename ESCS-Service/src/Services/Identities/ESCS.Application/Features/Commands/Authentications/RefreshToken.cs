using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Application.Extensions;
using ESCS.Application.Services;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Authentications
{
    internal class RefreshToken
    {
    }

    public class RefreshTokenCommand : ICommand<BaseResult<TokenResponse>>
    {
        public string RefreshToken { get; set; }

    }

    public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, BaseResult<TokenResponse>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(IUnitOfWork identityUnitOfWork, ITokenService tokenService, ILogger<RefreshTokenHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _tokenService = tokenService;
        }

        public async Task<BaseResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var refreshTokenRecord = await _unitOfWork.RefreshTokenRepository
                .FindEntityByQuery(t => t.Token == request.RefreshToken);


                if (refreshTokenRecord.IsRevoked || refreshTokenRecord is null || refreshTokenRecord.ExpiredDate <= DateTime.UtcNow)
                {
                    throw new AuthenticationException("Invalid or expired refresh token");
                }

                var user = await _unitOfWork.UserRepository.GetById(refreshTokenRecord.UserId);

                var role = await _unitOfWork.RoleRepository.GetById(user.RoleId);

                var roles = new List<string>
            {
                role.Name,
            };

                var accessToken = TokenExtension.CreateAccessToken(user, roles, false);

                await _tokenService.RevokeRefreshToken(request.RefreshToken);

                var refreshToken = await _tokenService.SaveRefreshToken(user.Id);

                var tokenResponse = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                };

                return BaseResult<TokenResponse>.Success(tokenResponse);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(RefreshTokenHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
