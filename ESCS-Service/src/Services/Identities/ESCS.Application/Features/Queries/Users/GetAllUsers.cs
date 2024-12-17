using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Users
{
    internal class GetAllUsers
    {
    }
    public class GetAllUsersQuery : IQuery<BaseResult<IEnumerable<UserGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllUsersHandler : IQueryHandler<GetAllUsersQuery, BaseResult<IEnumerable<UserGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersHandler> _logger;

        public GetAllUsersHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllUsersHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<UserGetDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var users = _mapper.Map<IEnumerable<UserGetDto>>
                (await _unitOfWork.UserRepository.FindByQuery(null, false, u => u.Role))
                ?? throw new NotFoundException($"User not found");

                return BaseResult<IEnumerable<UserGetDto>>.Success(users);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllUsersHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
