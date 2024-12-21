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
    internal class Login
    {
    }

    public class LoginCommand : ICommand<BaseResult<TokenResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class LoginHandler : ICommandHandler<LoginCommand, BaseResult<TokenResponse>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(IUnitOfWork identityUnitOfWork, ITokenService tokenService, ILogger<LoginHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _tokenService = tokenService;
        }

        //handling login
        public async Task<BaseResult<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            try
            {
                //get user by email
                var user = await _unitOfWork.UserRepository
               .FindEntityByQuery(u => u.Email == request.Email, false)
               ?? throw new AuthenticationException("User not found");

                //validate user infor
                if ((bool)user.LockoutEnabled)
                {
                    throw new AuthenticationException("User is locked");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    throw new AuthenticationException("Password is incorrect");
                }

                //get role of user
                var role = await _unitOfWork.RoleRepository.GetById(user.RoleId);

                var roles = new List<string>
            {
                role.Name,
            };

                //create access token
                var accessToken = TokenExtension.CreateAccessToken(user, roles, false);

                //create refreshtoken
                var refreshToken = await _tokenService.SaveRefreshToken(user.Id);

                var TokenResponse = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                };

                return BaseResult<TokenResponse>.Success(TokenResponse);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(LoginHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
