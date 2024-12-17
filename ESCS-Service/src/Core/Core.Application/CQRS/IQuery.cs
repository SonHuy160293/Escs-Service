using MediatR;

namespace Core.Application.CQRS
{
    // Define the IQuery interface with a generic type parameter that matches IRequest<TResponse>
    public interface IQuery<out TResponse> : IRequest<TResponse> { }

    // IQuery without type parameters can simply inherit from IQuery<Unit> for queries with no response
    public interface IQuery : IQuery<Unit> { }
}
