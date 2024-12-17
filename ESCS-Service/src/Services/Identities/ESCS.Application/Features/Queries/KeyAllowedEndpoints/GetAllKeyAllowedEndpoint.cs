using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Exceptions;
using ESCS.Application.Features.Queries.ApiKeys;
using ESCS.Domain.Dtos;
using ESCS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ESCS.Application.Features.Queries.KeyAllowedEndpoints
{
    internal class GetAllKeyAllowedEndpoint
    {
    }

    public class GetAllKeyAllowedEndpointsQuery : IQuery<BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>>
    {
    }

    // Implement the handler using the correct response type
    public class GetAllKeyAllowedEndpointsHandler : IQueryHandler<GetAllKeyAllowedEndpointsQuery, BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllKeyAllowedEndpointsHandler> _logger;

        public GetAllKeyAllowedEndpointsHandler(IUnitOfWork identityUnitOfWork, IMapper mapper, ILogger<GetAllKeyAllowedEndpointsHandler> logger)
        {
            _logger = logger;
            _unitOfWork = identityUnitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>> Handle(GetAllKeyAllowedEndpointsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var KeyAllowedEndpoints = _mapper.Map<IEnumerable<KeyAllowedEndpointGetDto>>
               (await _unitOfWork.KeyAllowedEndpointRepository.FindByQuery(null, false, u => u.ServiceEndpoint, u => u.UserApiKey))
               ?? throw new NotFoundException("Key allowed endpoint not found");

                return BaseResult<IEnumerable<KeyAllowedEndpointGetDto>>.Success(KeyAllowedEndpoints);
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception:{Exception}", typeof(GetAllUserApiKeysHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
