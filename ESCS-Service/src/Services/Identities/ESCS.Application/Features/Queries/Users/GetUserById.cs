using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Users
{
    internal class GetUserById
    {
    }

    public class GetUserByIdQuery : IQuery<BaseResult<UserGetDto>>
    {
        public long Id { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, BaseResult<UserGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdHandler> _logger;

        public GetUserByIdHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetUserByIdHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<UserGetDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var user = _mapper.Map<UserGetDto>
                (await _unitOfWork.UserRepository.FindEntityByQuery(u => u.Id == request.Id, false, u => u.Role))
                ?? throw new NotFoundException($"User with Id:{request.Id} not found");

                return BaseResult<UserGetDto>.Success(user);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetUserByIdHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
