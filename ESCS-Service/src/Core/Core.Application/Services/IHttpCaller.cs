using Core.Application.Common;

namespace Core.Application.Services
{
    public interface IHttpCaller
    {

        Task<BaseHttpResult<T>> GetAsync<T>(HttpCallOptions httpCallOptions) where T : class;
        Task<BaseHttpResult<TResponse>> PostAsync<TRequest, TResponse>(HttpCallOptions<TRequest> httpCallOptions)
           where TRequest : class;

    }
}
