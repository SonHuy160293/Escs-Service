using Core.Application.Common;
using Core.Infrastructure.DependencyModels;
using Emails.Domain.Interfaces;
using Emails.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Emails.Infrastructure.Persistences
{
    public class EmailRepository : IEmailRepository
    {

        private readonly MongoOptions _mongoOptions;
        private readonly IMongoCollection<Email> _emailCollection;
        private readonly ILogger<EmailRepository> _logger;
        public EmailRepository(IOptions<MongoOptions> mongoOptions,
            IMongoClient mongoClient, ILogger<EmailRepository> logger)
        {
            _mongoOptions = mongoOptions.Value;
            _logger = logger;
            _mongoOptions.CollectionName = typeof(Email).Name;

            var database = mongoClient.GetDatabase(_mongoOptions.DatabaseName);
            _emailCollection = database.GetCollection<Email>(_mongoOptions.CollectionName);
        }

        //add email to mongo
        public async Task<bool> AddEmailMessageAsync(Email email)
        {
            _logger.LogInformation("{Class} ADDING email with objectId:{ObjectId} to mongo", typeof(EmailRepository).Name, email.ObjectId);

            try
            {
                await _emailCollection.InsertOneAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} ADDING email with objectId:{ObjectId} to mongo catch exception:{Exception}", typeof(EmailRepository).Name, email.ObjectId, exception);

                throw;
            }
        }

        //retrive email from mongo by Id
        public async Task<Email?> GetEmailMessageByIdAsync(Guid id)
        {
            _logger.LogInformation("{Class} RETRIEVING email with id:{Id} to mongo", typeof(EmailRepository).Name, id);
            try
            {
                var filter = Builders<Email>.Filter.Eq(e => e.Id, id);
                return await _emailCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} RETRIEVING email with id:{Id} to mongo catch exception:{Exception}", typeof(EmailRepository).Name, id, exception);

                throw;
            }
        }

        //retrive email from mongo by objectId
        public async Task<Email?> GetEmailMessageByObjectIdAsync(string objectId)
        {
            _logger.LogInformation("{Class} RETRIEVING email with objectId:{ObjectId} to mongo", typeof(EmailRepository).Name, objectId);
            try
            {
                var filter = Builders<Email>.Filter.Eq(e => e.ObjectId, objectId);
                return await _emailCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                var exception = ExceptionError.Create(ex);
                _logger.LogInformation("{Class} RETRIEVING email with objectId:{ObjectId} to mongo catch exception:{Exception}", typeof(EmailRepository).Name, objectId, exception);

                throw;
            }
        }
    }
}
