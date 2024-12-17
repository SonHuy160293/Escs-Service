using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.Services
{
    class GetUserInEmailServiceWithPagination
    {
    }

    public class GetUserInEmailServiceWithPaginationQuery : IQuery<BasePagingResult<UserInServiceGetDto>>
    {
        public UserSearchRequest UserSearchRequest { get; set; }
    }

    // Implement the handler using the correct response type
    public class GetUserInEmailServiceWithPaginationHandler : IQueryHandler<GetUserInEmailServiceWithPaginationQuery, BasePagingResult<UserInServiceGetDto>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserInEmailServiceWithPaginationHandler> _logger;

        public GetUserInEmailServiceWithPaginationHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetUserInEmailServiceWithPaginationHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BasePagingResult<UserInServiceGetDto>> Handle(GetUserInEmailServiceWithPaginationQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var user = _mapper.Map<List<UserInServiceGetDto>>
                (await _unitOfWork.UserEmailServiceConfigurationRepository.FindByQuery
                        (u => u.User.Name.Contains(request.UserSearchRequest.UserName), false,
                   e => e.User)).DistinctBy(u => u.UserId).ToList();


                return BasePagingResult<UserInServiceGetDto>.Create(user, (int)request.UserSearchRequest.PageIndex, (int)request.UserSearchRequest.PageSize);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetUserInEmailServiceWithPaginationHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
