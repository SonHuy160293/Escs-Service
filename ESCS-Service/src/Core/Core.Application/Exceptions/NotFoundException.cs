namespace Core.Application.Exceptions
{

    public class NotFoundException : Exception
    {
        public string? ErrorCode { get; private set; }
        public NotFoundException(string errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public NotFoundException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
