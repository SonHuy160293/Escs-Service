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
    internal class CreateSystemToken
    {
    }

    public class CreateSystemTokenCommand : ICommand<BaseResult<TokenResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class CreateSystemTokenHandler : ICommandHandler<CreateSystemTokenCommand, BaseResult<TokenResponse>>
    {

        private readonly ILogger<CreateSystemTokenHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public CreateSystemTokenHandler(IUnitOfWork identityUnitOfWork, ITokenService tokenService, ILogger<CreateSystemTokenHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _tokenService = tokenService;
        }

        public async Task<BaseResult<TokenResponse>> Handle(CreateSystemTokenCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var user = await _unitOfWork.UserRepository
               .FindEntityByQuery(u => u.Email == request.Email, false)
               ?? throw new AuthenticationException("User not found");

                if ((bool)user.LockoutEnabled)
                {
                    throw new AuthenticationException("User is locked");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    throw new AuthenticationException("Password is incorrect");
                }

                if (user.Id != 2)
                {
                    throw new AuthenticationException("Not allowed");
                }

                var role = await _unitOfWork.RoleRepository.GetById(user.RoleId);

                var roles = new List<string>
            {
                role.Name,
            };

                var accessToken = TokenExtension.CreateAccessToken(user, roles, true);


                var TokenResponse = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = string.Empty
                };

                return BaseResult<TokenResponse>.Success(TokenResponse);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateSystemTokenHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
