namespace Emails.Application.Constants
{
    public static class RabbitMQConstant
    {
        public const string EmailCreatedQueueName = "email-created";
        public const string EmailSentQueueName = "email-sent";

        public const string EmailCreatedErrorQueueName = EmailCreatedQueueName + "_error";


        public const string EmailApplicationName = "Email";
    }
}
