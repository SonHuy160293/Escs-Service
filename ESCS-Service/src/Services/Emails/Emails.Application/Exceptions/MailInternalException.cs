using Core.Application.Exceptions;

namespace Emails.Application.Exceptions
{
    public class MailInternalServerException : InternalServerException
    {
        public MailInternalServerException(string message) : base(message)
        {
        }


    }
}
