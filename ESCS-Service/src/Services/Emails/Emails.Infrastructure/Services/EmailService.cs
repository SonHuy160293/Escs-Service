using Core.Application.Common;
using Core.Application.Extensions;
using Emails.Application.Exceptions;
using Emails.Application.Services;
using Emails.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using UserEmailConfiguration.Cache.Models;

namespace Emails.Infrastructure.Services
{
    public class EmailService : IEmailService
    {

        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(UserEmailConfig userEmailConfig, Email email)
        {
            _logger.LogInformation("{Class} SENDING email with id:{EmailId}", typeof(EmailService).Name, email.Id);

            // Create a new MailMessage
            using (MailMessage mail = new MailMessage())
            {
                // Set From address
                mail.From = new MailAddress(email.From);


                // Add To recipients
                foreach (var toEmail in email.To)
                {

                    mail.To.Add(toEmail);
                }

                // Add CC recipients
                foreach (var ccEmail in email.Cc)
                {
                    mail.CC.Add(ccEmail);
                }

                // Add BCC recipients
                foreach (var bccEmail in email.Bcc)
                {
                    mail.Bcc.Add(bccEmail);
                }

                // Set subject
                mail.Subject = email.Subject;

                // Add both plain text and HTML views for compatibility

                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.BodyHtml, null, "text/html"));

                if (email.Attachments is not null && email.Attachments.Any())
                {
                    // Attach each file from the byte array.
                    foreach (var filePath in email.Attachments)
                    {

                        var attachment = new System.Net.Mail.Attachment(filePath);

                        // Set a new name for the file
                        attachment.Name = Extension.ExtractFileName(filePath); // Specify the desired file name and extension

                        mail.Attachments.Add(attachment);
                    }
                }

                // SMTP client configuration
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", userEmailConfig.SmtpPort))
                {
                    smtp.Credentials = new NetworkCredential(userEmailConfig.SmtpEmail, userEmailConfig.SmtpPassword);
                    smtp.EnableSsl = true; // Enable SSL if required by your SMTP server

                    try
                    {
                        //send mail
                        await smtp.SendMailAsync(mail);
                    }
                    catch (Exception ex)
                    {

                        var exception = ExceptionError.Create(ex);
                        _logger.LogInformation("{Class} SENDING email with objectId:{ObjectId} catch exception:{Exception}", typeof(EmailService).Name, email.ObjectId, exception);

                        throw new MailInternalServerException(ex.Message);
                    }
                }
            }


        }
    }
}
