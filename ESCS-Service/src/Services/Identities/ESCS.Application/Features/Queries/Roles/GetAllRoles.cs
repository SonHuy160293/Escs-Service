using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Roles
{
    internal class GetAllRoles
    {
    }

    public class GetAllRolesQuery : IQuery<BaseResult<IEnumerable<RoleGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllRolesHandler : IQueryHandler<GetAllRolesQuery, BaseResult<IEnumerable<RoleGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllRolesHandler> _logger;

        public GetAllRolesHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllRolesHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IEnumerable<RoleGetDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var roles = _mapper.Map<IEnumerable<RoleGetDto>>
                (await _unitOfWork.RoleRepository.FindByQuery(null, false))
                ?? throw new NotFoundException($"Role not found");

                return BaseResult<IEnumerable<RoleGetDto>>.Success(roles);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllRolesHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
