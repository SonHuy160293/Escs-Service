using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using ESCS.Application.Exceptions;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Users
{
    public class CreateUserCommand : ICommand<BaseResult>
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;

        public long RoleId { get; set; }

    }


    public class CreateUserHandler : ICommandHandler<CreateUserCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateUserHandler> _logger;
        public CreateUserHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<CreateUserHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.FindByQuery(u => u.Email == request.Email);

                if (user.Any())
                {
                    throw new ExistException("User email existed");
                }

                var userCreated = _mapper.Map<User>(request);
                userCreated.LockoutEnabled = false;
                userCreated.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                await _unitOfWork.UserRepository.Add(userCreated);
                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateUserHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
