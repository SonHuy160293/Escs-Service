using AutoMapper;
using Core.Application.Common;
using Core.Application.CQRS;
using Core.Application.Helpers;
using Core.Application.Services;
using Emails.Application.Constants;
using Emails.Domain.Events;
using Emails.Domain.Interfaces;
using Emails.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserEmailConfiguration.Cache.Interfaces;

namespace Emails.Application.Features.Emails
{
    internal class SendEmail
    {
    }

    public class SendEmailCommand : ICommand<BaseResult>
    {
        public string? From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string? Subject { get; set; }
        public string? BodyText { get; set; }
        public string? BodyHtml { get; set; }
        public bool IsNoReply { get; set; }
        public List<IFormFile>? Files { get; set; } = default!;
        public string ObjectId { get; set; } = default!;
        public string? UrlCallback { get; set; }
        public string? AuthenticationType { get; set; }
        public string? Key { get; set; }

    }

    public class SendEmailHandler : ICommandHandler<SendEmailCommand, BaseResult>
    {

        private readonly ICachedUserEmailConfigRepository _cachedUserEmailConfigRepository;

        private readonly IMassTransitHandler _massTransitHandler;
        private readonly IFileService _fileService;
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<SendEmailHandler> _logger;

        private readonly IMapper _mapper;


        public SendEmailHandler(IFileService fileService,
            IEmailRepository emailRepository,
            IMassTransitHandler massTransitHandler, ILogger<SendEmailHandler> logger,
            ICachedUserEmailConfigRepository cachedUserEmailConfigRepository, IMapper mapper
            )
        {

            _fileService = fileService;
            _emailRepository = emailRepository;
            _massTransitHandler = massTransitHandler;
            _logger = logger;
            _cachedUserEmailConfigRepository = cachedUserEmailConfigRepository;
            _mapper = mapper;

        }

        public async Task<BaseResult> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var filesPath = new List<string>();

                if (request.Files is not null && request.Files.Any())
                {
                    filesPath = await _fileService.WriteFile(request.Files);
                }

                var email = _mapper.Map<Email>(request);
                email.Attachments = filesPath;

                await _emailRepository.AddEmailMessageAsync(email);

                await _massTransitHandler.Send(RabbitMQConstant.EmailCreatedQueueName, new EmailCreatedEvent()
                {
                    EmailId = email.Id,
                    ObjectId = email.ObjectId,
                    CorrelationId = CorrelationIdProvider.GetCorrelationId()
                });

                return BaseResult.Success();
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("{Class} catch exception {Exception}", typeof(SendEmailHandler).Name, exceptionError);
                throw;
            }
        }
    }
}
