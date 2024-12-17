using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using ESCS.Domain.Interfaces;
using ESCS.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Commands.Roles
{
    internal class CreateRole
    {
    }

    public class CreateRoleCommand : ICommand<BaseResult>
    {
        public string Name { get; set; } = default!;

    }

    public class CreateRoleHandler : ICommandHandler<CreateRoleCommand, BaseResult>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateRoleHandler> _logger;

        public CreateRoleHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateRoleHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var role = _mapper.Map<Role>(request);

                await _unitOfWork.RoleRepository.Add(role);

                await _unitOfWork.SaveChangesAsync();

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(CreateRoleHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
