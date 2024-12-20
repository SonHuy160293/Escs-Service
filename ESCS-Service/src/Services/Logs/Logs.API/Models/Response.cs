using System.Text.Json.Serialization;

namespace Logs.API.Models
{
    public class Response
    {
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class LogRecord
    {

        public string Id { get; set; }

        [JsonPropertyName("@timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }


        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("fields")]
        public Dictionary<string, object> Fields { get; set; }
    }

    public class StatisticLogResponse
    {
        public long Total { get; set; }
        public Dictionary<string, long> Data { get; set; }
    }

    public class StatisticLogDetailResponse
    {
        public long Total { get; set; }
        public Dictionary<string, DataDetail> Data { get; set; }
    }

    public class DataDetail
    {
        public long TotalResponse { get; set; }
        public double AverageProcessedTime { get; set; }


    }

    public class UserRegisterInServiceResponse
    {
        public IEnumerable<long> UserIds { get; set; }
    }

    public class LogField
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}