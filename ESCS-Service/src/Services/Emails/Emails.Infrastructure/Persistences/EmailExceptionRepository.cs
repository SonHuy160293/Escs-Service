using Core.Application.Common;
using Core.Infrastructure.DependencyModels;
using Emails.Domain.Interfaces;
using Emails.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Emails.Infrastructure.Persistences
{
    public class EmailExceptionRepository : IEmailExceptionRepository
    {
        private readonly MongoOptions _mongoOptions;
        private readonly IMongoCollection<EmailException> _emailExceptionCollection;
        private readonly ILogger<EmailExceptionRepository> _logger;
        public EmailExceptionRepository(IOptions<MongoOptions> mongoOptions,
            IMongoClient mongoClient, ILogger<EmailExceptionRepository> logger)
        {
            _mongoOptions = mongoOptions.Value;
            _logger = logger;
            _mongoOptions.CollectionName = "Email_Exception";

            var database = mongoClient.GetDatabase(_mongoOptions.DatabaseName);
            _emailExceptionCollection = database.GetCollection<EmailException>(_mongoOptions.CollectionName);
        }

        //add emailException to mongo
        public async Task<bool> AddEmailExceptionAsync(EmailException emailException)
        {


            _logger.LogInformation("{Class} ADDING emailException with id:{Id} to mongo", typeof(EmailExceptionRepository).Name, emailException.MessageId);

            try
            {
                await _emailExceptionCollection.InsertOneAsync(emailException);
                return true;
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} ADDING emailException with with id:{Id} to mongo catch exception:{Exception}", typeof(EmailExceptionRepository).Name, emailException.MessageId, exception);
                throw;
            }

        }

        //retrieve emailException from mongo
        public async Task<EmailException> GetEmailExceptionByIdAsync(Guid id)
        {

            _logger.LogInformation("{Class} RETRIEVING emailException with id:{Id} to mongo", typeof(EmailExceptionRepository).Name, id);

            try
            {
                var filter = Builders<EmailException>.Filter.Eq(e => e.MessageId, id);
                return await _emailExceptionCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} RETRIEVING emailException with with id:{Id} to mongo catch exception:{Exception}", typeof(EmailExceptionRepository).Name, id, exception);

                throw;
            }

        }
    }
}
