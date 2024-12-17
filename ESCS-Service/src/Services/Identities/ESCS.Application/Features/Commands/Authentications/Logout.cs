using Core.Application.Common;
using Core.Application.CQRS;
using ESCS.Application.Services;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Authentications
{
    internal class Logout
    {
    }

    public class LogoutCommand : ICommand<BaseResult>
    {
        public string RefreshToken { get; set; }

    }

    public class LogoutHandler : ICommandHandler<LogoutCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LogoutHandler> _logger;

        public LogoutHandler(IUnitOfWork identityUnitOfWork, ITokenService tokenService, ILogger<LogoutHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _tokenService = tokenService;
        }

        public async Task<BaseResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {

                await _tokenService.RevokeRefreshToken(request.RefreshToken);

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(LogoutHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
