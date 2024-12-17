namespace Core.Application.Exceptions
{
    public class InternalServerException : Exception
    {
        public string? ErrorCode { get; private set; }
        public InternalServerException(string errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public InternalServerException(string errorMessage) : base(errorMessage)
        {

        }
    }
}

