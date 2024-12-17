using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Roles
{
    internal class GetRoleById
    {
    }

    public class GetRoleByIdQuery : IQuery<BaseResult<RoleGetDto>>
    {
        public long Id { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetRoleByIdHandler : IQueryHandler<GetRoleByIdQuery, BaseResult<RoleGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRoleByIdHandler> _logger;

        public GetRoleByIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetRoleByIdHandler> logger)
        {
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<RoleGetDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var role = await _unitOfWork.RoleRepository.FindEntityByQuery(r => r.Id == request.Id, false)
                ?? throw new NotFoundException($"Role with Id:{request.Id} not found");


                return BaseResult<RoleGetDto>.Success(_mapper.Map<RoleGetDto>(role));
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetRoleByIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
