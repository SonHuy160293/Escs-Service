using Core.Application.Exceptions;

namespace ESCS.Application.Exceptions
{
    public class ExistException : BusinessException
    {
        public ExistException(string errorMessage) : base(errorMessage)
        {
        }

        public ExistException(string errorCode, string errorMessage) : base(errorCode, errorMessage)
        {
        }
    }
}
