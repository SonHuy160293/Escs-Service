using MediatR;

namespace Core.Application.CQRS
{

    public interface ICommand : ICommand<Unit>
    {

    }

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
