using Core.Application.Services;
using Core.Infrastructure.Services;
using Emails.Application.Services;
using Emails.Domain.Events;
using Emails.Domain.Interfaces;
using MassTransit;
using UserEmailConfiguration.Cache.Interfaces;

namespace Emails.Created.Consumer.Consume
{
    public class EmailCreatedConsumer : BaseConsumer<EmailCreatedEvent>
    {

        private readonly IEmailService _emailService;
        private readonly IEmailRepository _emailRepository;

        private readonly ICachedUserEmailConfigRepository _cachedUserEmailConfigRepository;
        private readonly IHttpCaller _httpCaller;
        private readonly IIdentityService _identityService;
        private readonly ILogger<EmailCreatedConsumer> _logger;
        private readonly IIdentityGrpcService _identityGrpcService;

        public EmailCreatedConsumer(IEmailService emailService, IEmailRepository emailRepository,
            ILogger<EmailCreatedConsumer> logger, ICachedUserEmailConfigRepository cachedUserEmailConfigRepository,
            IHttpCaller httpCaller, IIdentityService identityService, IIdentityGrpcService identityGrpcService)
        {
            _emailService = emailService;
            _emailRepository = emailRepository;

            _logger = logger;
            _cachedUserEmailConfigRepository = cachedUserEmailConfigRepository;
            _httpCaller = httpCaller;
            _identityService = identityService;
            _identityGrpcService = identityGrpcService;
        }

        protected override async Task HandleConsume(ConsumeContext<EmailCreatedEvent> context)
        {
            _logger.LogInformation("Consumer:{Consumer} consuming message with emailId:{EmailID}", typeof(EmailCreatedConsumer).Name, context.Message.EmailId);

            //var email = await _emailRepository.GetEmailMessageByIdAsync(context.Message.EmailId);

            //var userEmailConfig = await _cachedUserEmailConfigRepository.GetUserEmailConfig(email.From);

            //if (userEmailConfig is null)
            //{
            //    userEmailConfig = await _identityGrpcService.GetUserEmailConfig(email.From);

            //    await _cachedUserEmailConfigRepository.AddUserEmailConfig(userEmailConfig);
            //}


            //if (email is not null)
            //{
            //    await _emailService.SendEmailAsync(userEmailConfig, email);


            //    //call back
            //    if (!string.IsNullOrEmpty(email.UrlCallback))
            //    {

            //        string receiverEmails = string.Join(",", email.To);

            //        var requestBody = new CalbackRequestBody()
            //        {
            //            IsSent = true,
            //            Message = $"Successfully sent mail to {receiverEmails}",
            //            ObjectId = context.Message.ObjectId,
            //        };

            //        //check if callback url required authentication
            //        var isCallBackAuthenticated = !string.IsNullOrEmpty(email.AuthenticationType) && !string.IsNullOrEmpty(email.Key);

            //        //declare httpCallOptions: isDeserialized, isRetry, requestBody, callBackUrl, authenticationType, key
            //        var httpCallOptions = isCallBackAuthenticated ?
            //              HttpCallOptions<CalbackRequestBody>.Authenticated(isSerialized: false, isRetry: true, requestBody, email.UrlCallback, null, email.AuthenticationType, email.Key) :
            //              HttpCallOptions<CalbackRequestBody>.UnAuthenticated(isSerialized: false, isRetry: true, requestBody, email.UrlCallback, null);

            //        //send request to callback url
            //        var response = await _httpCaller.PostAsync<CalbackRequestBody, BaseHttpResult>(httpCallOptions);
            //    }

            //}
            //else
            //{
            //    throw new Exception("Can not find message");
            //}
        }
    }
}
