namespace ESCS.Application.Exceptions
{
    public class DuplicateException : Exception
    {
        public string? ErrorCode { get; private set; }
        public DuplicateException(string errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public DuplicateException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
