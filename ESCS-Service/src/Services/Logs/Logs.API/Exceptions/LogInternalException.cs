using Core.Application.Exceptions;

namespace Logs.API.Exceptions
{
    public class LogInternalException : InternalServerException
    {
        public LogInternalException(string message) : base(message)
        {
        }


    }
}
