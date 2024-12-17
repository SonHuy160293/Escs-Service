namespace Core.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public string? ErrorCode { get; private set; }
        public BusinessException(string errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string errorMessage) : base(errorMessage)
        {

        }
    }
}

