using Core.Domain.Base;

namespace Logs.API.Models
{
    public class Request
    {
    }


    public class SearchLogByUserIdRequest : BaseSearchRequest
    {
        public string Index { get; set; }
        public long UserId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? RequestPath { get; set; }
        public string Environment { get; set; }

        public bool? IsSuccessStatusCode { get; set; }
    }

    public class FilterLogResponseRequestModel
    {
        public long UserId { get; set; }
        public string FilterDate { get; set; }
        public string FilterType { get; set; }
        public string Environment { get; set; }
        public string? RequestPath { get; set; }
        public string Index { get; set; }
        public bool? IsSuccessStatusCode { get; set; }
    }

    public class FilterLogResponseDetailRequestModel
    {
        public long UserId { get; set; }
        public string FilterDate { get; set; }
        public string FilterType { get; set; }
        public string Environment { get; set; }
        public string? RequestPath { get; set; }
        public string? Method { get; set; }
        public string Index { get; set; }
        public string? StatusCode { get; set; }
    }

    public class FilterServiceUsagelRequestModel
    {

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Environment { get; set; }
        public string RequestPath { get; set; }
        public string Method { get; set; }
        public string Index { get; set; }
        public string? StatusCode { get; set; }
    }


    public class Filter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class LogSearchRequest : BaseSearchRequest
    {
        public string Index { get; set; }
        public string SearchTerm { get; set; }
        public List<Filter> Filters { get; set; } = new List<Filter>();
        // Add StartDate and EndDate properties
        public string StartDate { get; set; }  // Preferably use string if you're receiving date as string from frontend
        public string EndDate { get; set; }    // Alternatively, you can use DateTime?
        public bool IsLatest { get; set; }
    }
}
