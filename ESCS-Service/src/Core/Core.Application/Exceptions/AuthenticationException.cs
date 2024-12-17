namespace Core.Application.Exceptions
{


    public class AuthenticationException : Exception
    {
        public string? ErrorCode { get; private set; }
        public AuthenticationException(string errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public AuthenticationException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
