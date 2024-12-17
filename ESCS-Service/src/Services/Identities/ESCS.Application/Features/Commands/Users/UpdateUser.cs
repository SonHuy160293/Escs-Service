using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Users
{
    internal class UpdateUser
    {
    }

    public class UpdateUserCommand : ICommand<BaseResult>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;



    }


    public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserHandler> _logger;
        public UpdateUserHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<UpdateUserHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.FindEntityByQuery(u => u.Id == request.Id);

                if (user is null)
                {
                    throw new NotFoundException("User not found");
                }

                user.PhoneNumber = request.PhoneNumber;
                user.Name = request.Name;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(UpdateUserHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
