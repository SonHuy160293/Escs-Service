namespace Core.Application.Common
{
    public class ExceptionError
    {
        public int StatusCode { get; private set; }
        public string Message { get; private set; }
        public string InnerException { get; private set; }
        public string StackTrace { get; private set; }

        private ExceptionError(int statusCode, string message, string innerException, string stackTrace)
        {
            StatusCode = statusCode;
            Message = message;
            InnerException = innerException;
            StackTrace = stackTrace;
        }

        public static ExceptionError Create(int statusCode, string message, string innerException, string stackTrace)
        {
            return new ExceptionError(statusCode, message, innerException, stackTrace);
        }

        public static ExceptionError Create(Exception ex)
        {
            return new ExceptionError(0, ex.Message, ex.InnerException?.ToString() ?? string.Empty, ex.StackTrace ?? string.Empty);
        }
    }
}
