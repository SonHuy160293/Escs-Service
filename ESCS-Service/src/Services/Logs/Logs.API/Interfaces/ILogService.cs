using Logs.API.Models;

namespace Logs.API.Interfaces
{
    public interface ILogService
    {
        public Task<IEnumerable<string>> GetAllFields(string index);

        public Task<PaginatedResponse<LogRecord>> GetResponseLogByUser(SearchLogByUserIdRequest searchLogByUserIdRequest);

        public Task<LogRecord> GetLogById(string indexName, string logId);

        public Task<StatisticLogResponse> GetUserLogResponseStatistic(FilterLogResponseRequestModel requestModel);

        public Task<StatisticLogDetailResponse> GetUserLogResponseDetailStatistic(FilterLogResponseDetailRequestModel requestModel);

        public Task<StatisticLogDetailResponse> GetServiceUsageStatistic(FilterServiceUsagelRequestModel requestModel);
        public Task<PaginatedResponse<LogRecord>> GetLogsPaging(LogSearchRequest logSearchRequest);
        public Task<IEnumerable<(string FieldName, string FieldType)>> GetAllFieldsWithTypes(string index);
        public Task<LogRecord> GetLogResponseByCorrelationId(string index, string correlationId);

    }
}
