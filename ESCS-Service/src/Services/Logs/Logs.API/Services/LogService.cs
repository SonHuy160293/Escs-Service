using Core.Application.Common;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Identity.Cache.Interfaces;
using Identity.Cache.Models;
using Logs.API.Exceptions;
using Logs.API.Extensions;
using Logs.API.Interfaces;
using Logs.API.Models;
using MassTransit.Initializers;
using System.Globalization;



namespace Logs.API.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly ElasticsearchClient _elasticSearchClient;
        private readonly IIdentityGrpcService _identityGrpcService;
        private readonly ICachedEndpointUserRepository _cachedEndpointUserRepository;

        public LogService(ILogger<LogService> logger, ElasticsearchClient elasticSearchClient
            , IIdentityGrpcService identityGrpcService, ICachedEndpointUserRepository cachedEndpointUserRepository)
        {
            _logger = logger;
            _elasticSearchClient = elasticSearchClient;
            _identityGrpcService = identityGrpcService;
            _cachedEndpointUserRepository = cachedEndpointUserRepository;
        }

        public async Task<IEnumerable<string>> GetAllFields(string index)
        {
            try
            {
                // Replace "your-index-name" with the actual name of your index
                var response = await _elasticSearchClient.Indices.GetMappingAsync(new GetMappingRequest(index));

                if (!response.IsValidResponse || !response.Indices.Any())
                {
                    throw new LogInternalException("Failed to retrieve index mapping.");
                }

                // Assuming you have a single index
                var indexMapping = response.Indices.First().Value;

                // Get the properties dictionary
                var properties = indexMapping.Mappings.Properties;

                // Flatten the properties to get all field paths
                var fields = GetAllFields(properties);

                return fields;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("Retrive fields catch exception {Exception}", exceptionError);

                throw;
            }
        }

        public async Task<PaginatedResponse<LogRecord>> GetResponseLogByUser(SearchLogByUserIdRequest searchLogByUserIdRequest)
        {
            try
            {
                // Build the query
                var mustQueries = new List<Query>();

                mustQueries.Add(new TermQuery(field: "fields.LogType.raw")
                {
                    Value = "Response"
                });

                mustQueries.Add(new TermQuery(field: "fields.UserId")
                {
                    Value = searchLogByUserIdRequest.UserId
                });

                mustQueries.Add(new TermQuery(field: "fields.Environment.raw")
                {
                    Value = searchLogByUserIdRequest.Environment
                });

                if (!string.IsNullOrEmpty(searchLogByUserIdRequest.RequestPath))
                {
                    mustQueries.Add(new TermQuery(field: "fields.RequestPath.raw")
                    {
                        Value = searchLogByUserIdRequest.RequestPath
                    });
                }

                // Date range filter
                // Date range filter
                if (!string.IsNullOrEmpty(searchLogByUserIdRequest.FromDate) || !string.IsNullOrEmpty(searchLogByUserIdRequest.ToDate))
                {
                    var dateRangeQuery = new DateRangeQuery(field: "@timestamp");

                    // Adjusted format string for parsing
                    var format = "dd-MM-yyyy HH:mm:ss";
                    var culture = CultureInfo.InvariantCulture;

                    // Try parsing fromDate
                    if (!string.IsNullOrEmpty(searchLogByUserIdRequest.FromDate) &&
                        DateTime.TryParseExact(searchLogByUserIdRequest.FromDate, format, culture, DateTimeStyles.None, out var startDate))
                    {
                        // Convert to UTC if necessary
                        dateRangeQuery.Gte = DateTime.SpecifyKind(startDate, DateTimeKind.Local).ToUniversalTime();
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse FromDate");
                    }

                    // Try parsing toDate
                    if (!string.IsNullOrEmpty(searchLogByUserIdRequest.ToDate) &&
                        DateTime.TryParseExact(searchLogByUserIdRequest.ToDate, format, culture, DateTimeStyles.None, out var endDate))
                    {
                        // Convert to UTC if necessary
                        dateRangeQuery.Lte = DateTime.SpecifyKind(endDate, DateTimeKind.Local).ToUniversalTime();
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse ToDate");
                    }

                    mustQueries.Add(dateRangeQuery);
                }


                if (searchLogByUserIdRequest.IsSuccessStatusCode.HasValue)
                {
                    var statusCodeRangeQuery = new NumberRangeQuery(field: "fields.StatusCode");

                    if ((bool)searchLogByUserIdRequest.IsSuccessStatusCode)
                    {

                        statusCodeRangeQuery.Gte = 200;
                        statusCodeRangeQuery.Lte = 299;
                    }
                    else
                    {
                        statusCodeRangeQuery.Gte = 300;
                        statusCodeRangeQuery.Lte = 199;
                    }
                    mustQueries.Add(statusCodeRangeQuery);
                }



                var query = new BoolQuery { Must = mustQueries };

                // Execute the search
                var response = await _elasticSearchClient.SearchAsync<LogRecord>(s => s
                                            .Index(searchLogByUserIdRequest.Index)
                                            .Query(query)
                                            .Sort(r => r.Field("@timestamp", new FieldSort
                                            {
                                                Order = SortOrder.Desc
                                            }))
                                            .From(searchLogByUserIdRequest.PageIndex * searchLogByUserIdRequest.PageSize)
                                            .Size(searchLogByUserIdRequest.PageSize)
                                            .SourceIncludes(new[]
                                                                 {
                                                                 "@timestamp",
                                                                 "fields.CorrelationId",
                                                                 "fields.RequestPath",
                                                                 "level",
                                                                 "message",
                                                                 "fields.Method",
                                                                 "fields.StatusCode"
                                                                  })
                                            );



                var logRecords = response.Hits.Select(hit => new LogRecord
                {
                    Id = hit.Id, // Extract _id from the hit metadata
                    Timestamp = hit.Source.Timestamp,
                    Level = hit.Source.Level,
                    Message = hit.Source.Message,
                    Fields = hit.Source.Fields
                }).ToList();


                if (!response.IsValidResponse)
                {
                    throw new LogInternalException("Elasticsearch query failed.");
                }

                var countResponse = await _elasticSearchClient.CountAsync<LogRecord>(c => c.Indices(searchLogByUserIdRequest.Index).Query(query));

                var totalCount = countResponse.Count;

                var result = new PaginatedResponse<LogRecord>
                {
                    Items = logRecords,
                    PageIndex = (int)searchLogByUserIdRequest.PageIndex,
                    PageSize = (int)searchLogByUserIdRequest.PageSize,
                    TotalCount = (int)totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)searchLogByUserIdRequest.PageSize),
                    HasPreviousPage = searchLogByUserIdRequest.PageIndex > 0,
                    HasNextPage = searchLogByUserIdRequest.PageIndex < (int)Math.Ceiling(totalCount / (double)searchLogByUserIdRequest.PageSize) - 1
                };

                return result;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                _logger.LogError("GetResponseLogByUser catch exception {Exception}", exceptionError);

                throw;
            }

        }

        // Helper method to flatten properties and get all field paths
        private List<string> GetAllFields(Properties properties, string parentPath = "")
        {
            try
            {
                var fields = new List<string>();

                foreach (var prop in properties)
                {
                    var fieldName = parentPath + prop.Key;

                    if (prop.Value is ObjectProperty objectProp && objectProp.Properties != null)
                    {
                        // Recursively get nested fields
                        var nestedFields = GetAllFields(objectProp.Properties, fieldName + ".");
                        fields.AddRange(nestedFields);
                    }
                    else
                    {
                        fields.Add(fieldName);
                    }
                }

                return fields;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetAllFields catch {exceptionError}");
            }
        }

        public async Task<LogRecord> GetLogById(string indexName, string logId)
        {
            try
            {
                if (string.IsNullOrEmpty(indexName)) throw new ArgumentNullException(nameof(indexName));
                if (string.IsNullOrEmpty(logId)) throw new ArgumentNullException(nameof(logId));


                // Check if the index exists
                var indexExistsResponse = await _elasticSearchClient.Indices.ExistsAsync(indexName);

                if (!indexExistsResponse.IsValidResponse || !indexExistsResponse.Exists)
                {
                    throw new Exception($"The index '{indexName}' does not exist.");
                }

                // Perform the Get operation
                var response = await _elasticSearchClient.GetAsync<LogRecord>(logId, g => g.Index(indexName));

                if (!response.IsValidResponse || response.Source == null)
                {
                    // Handle invalid response or missing source
                    throw new Exception($"Log with ID '{logId}' not found in index '{indexName}'.");
                }

                // Return the retrieved log record
                return new LogRecord
                {
                    Id = response.Id,
                    Timestamp = response.Source.Timestamp,
                    Level = response.Source.Level,
                    Message = response.Source.Message,
                    Fields = response.Source.Fields
                };
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetLogById catch {exceptionError}");
            }
        }

        public async Task<StatisticLogResponse> GetUserLogResponseStatistic(FilterLogResponseRequestModel requestModel)
        {
            try
            {
                long count = 0;

                // Build the query
                var mustQueries = new List<Query>();

                mustQueries.Add(new TermQuery(field: "fields.LogType.raw")
                {
                    Value = "Response"
                });

                mustQueries.Add(new TermQuery(field: "fields.UserId")
                {
                    Value = requestModel.UserId
                });

                mustQueries.Add(new TermQuery(field: "fields.Environment.raw")
                {
                    Value = requestModel.Environment
                });

                if (!string.IsNullOrEmpty(requestModel.RequestPath))
                {
                    mustQueries.Add(new TermQuery(field: "fields.RequestPath.raw")
                    {
                        Value = requestModel.RequestPath
                    });
                }

                if (requestModel.IsSuccessStatusCode.HasValue)
                {
                    var statusCodeRangeQuery = new NumberRangeQuery(field: "fields.StatusCode");

                    if (requestModel.IsSuccessStatusCode.Value)
                    {
                        statusCodeRangeQuery.Gte = 200;
                        statusCodeRangeQuery.Lte = 299;
                    }
                    else
                    {
                        statusCodeRangeQuery.Gte = 300;
                        statusCodeRangeQuery.Lte = 199;
                    }
                    mustQueries.Add(statusCodeRangeQuery);
                }

                var dateRangeQuery = new DateRangeQuery(field: "@timestamp");

                Dictionary<string, long> statistics = new Dictionary<string, long>();

                var format = "dd-MM-yyyy HH:mm:ss";
                var culture = CultureInfo.InvariantCulture;

                DateTime.TryParseExact(requestModel.FilterDate, format, culture, DateTimeStyles.None, out var date);

                switch (requestModel.FilterType)
                {
                    case "year":
                        var months = TimeExtension.GetMonthsInYear(date.Year);

                        foreach (var month in months)
                        {
                            // Get the first day of the month
                            DateTime startDate = new DateTime(date.Year, month, 1);

                            // Get the last day of the month by adding 1 month and subtracting 1 day
                            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                            // Convert to UTC if necessary
                            dateRangeQuery.Gte = DateTime.SpecifyKind(startDate, DateTimeKind.Local).ToUniversalTime();

                            // Convert to UTC if necessary
                            dateRangeQuery.Lte = DateTime.SpecifyKind(endDate, DateTimeKind.Local).ToUniversalTime();

                            mustQueries.Add(dateRangeQuery);

                            var query = new BoolQuery { Must = mustQueries };

                            var countRequest = new CountRequest(requestModel.Index) { Query = query };

                            var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                            statistics.Add(month.ToString(), countResponse.Count);

                            count += countResponse.Count;
                        }
                        break;

                    case "month":
                        var days = TimeExtension.GetDaysInMonth(date.Year, date.Month);
                        foreach (var day in days)
                        {
                            // Parse the input string into a DateTime object
                            DateTime dateTime = DateTime.ParseExact(day, "dd/MM", CultureInfo.InvariantCulture);

                            // Start time is at midnight (00:00:00)
                            DateTime startTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

                            // End time is at the last moment of the day (23:59:59)
                            DateTime endTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

                            // Convert to UTC if necessary
                            dateRangeQuery.Gte = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();

                            // Convert to UTC if necessary
                            dateRangeQuery.Lte = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                            mustQueries.Add(dateRangeQuery);

                            var query = new BoolQuery { Must = mustQueries };

                            var countRequest = new CountRequest(requestModel.Index) { Query = query };

                            var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                            statistics.Add(day, countResponse.Count);

                            count += countResponse.Count;
                        }
                        break;

                    case "day":
                        var hours = TimeExtension.GetHoursInday(date);
                        foreach (var hour in hours)
                        {
                            // Start time is at midnight (00:00:00)
                            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);

                            // End time is at the last moment of the day (23:59:59)
                            DateTime endTime = new DateTime(date.Year, date.Month, date.Day, hour, 59, 59);

                            // Convert to UTC if necessary
                            dateRangeQuery.Gte = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();

                            // Convert to UTC if necessary
                            dateRangeQuery.Lte = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                            mustQueries.Add(dateRangeQuery);

                            var query = new BoolQuery { Must = mustQueries };

                            var countRequest = new CountRequest(requestModel.Index) { Query = query };

                            var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                            statistics.Add(hour.ToString(), countResponse.Count);

                            count += countResponse.Count;
                        }
                        break;

                    default:
                        throw new ArgumentException($"Invalid filter type: {requestModel.FilterType}");
                }

                return new StatisticLogResponse
                {
                    Data = statistics,
                    Total = count,
                };
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetUserLogResponseStatistic catch {exceptionError}");
            }
        }

        public async Task<StatisticLogDetailResponse> GetUserLogResponseDetailStatistic(FilterLogResponseDetailRequestModel requestModel)
        {
            try
            {
                long count = 0;

                // Build the query
                var mustQueries = new List<Query>();

                mustQueries.Add(new TermQuery(field: "fields.LogType.raw")
                {
                    Value = "Response"
                });

                mustQueries.Add(new TermQuery(field: "fields.UserId")
                {
                    Value = requestModel.UserId
                });

                mustQueries.Add(new TermQuery(field: "fields.Environment.raw")
                {
                    Value = requestModel.Environment
                });

                if (!string.IsNullOrEmpty(requestModel.RequestPath) && !string.IsNullOrEmpty(requestModel.Method))
                {
                    mustQueries.Add(new TermQuery(field: "fields.RequestPath.raw")
                    {
                        Value = requestModel.RequestPath
                    });

                    mustQueries.Add(new TermQuery(field: "fields.Method.raw")
                    {
                        Value = requestModel.Method
                    });
                }

                if (!string.IsNullOrEmpty(requestModel.StatusCode))
                {
                    var statusCodeRangeQuery = new NumberRangeQuery(field: "fields.StatusCode");


                    switch (requestModel.StatusCode)
                    {
                        case "1XX":

                            statusCodeRangeQuery.Gte = 100;
                            statusCodeRangeQuery.Lte = 199;
                            break;

                        case "2XX":

                            statusCodeRangeQuery.Gte = 200;
                            statusCodeRangeQuery.Lte = 299;
                            break;

                        case "3XX":

                            statusCodeRangeQuery.Gte = 300;
                            statusCodeRangeQuery.Lte = 399;
                            break;

                        case "4XX":

                            statusCodeRangeQuery.Gte = 400;
                            statusCodeRangeQuery.Lte = 499;
                            break;

                        case "5XX":

                            statusCodeRangeQuery.Gte = 500;
                            statusCodeRangeQuery.Lte = 599;
                            break;
                    }


                    mustQueries.Add(statusCodeRangeQuery);
                }

                var dateRangeQuery = new DateRangeQuery(field: "@timestamp");

                Dictionary<string, DataDetail> statistics = new Dictionary<string, DataDetail>();

                var format = "dd-MM-yyyy HH:mm:ss";
                var culture = CultureInfo.InvariantCulture;

                DateTime.TryParseExact(requestModel.FilterDate, format, culture, DateTimeStyles.None, out var date);

                switch (requestModel.FilterType)
                {
                    case "year":
                        var months = TimeExtension.GetMonthsInYear(date.Year);

                        foreach (var month in months)
                        {


                            // Get the first day of the month
                            DateTime startMonth = new DateTime(date.Year, month, 1);

                            // Get the last day of the month by adding 1 month and subtracting 1 day
                            DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);

                            // Convert to UTC if necessary
                            dateRangeQuery.Gte = DateTime.SpecifyKind(startMonth, DateTimeKind.Local).ToUniversalTime();

                            // Convert to UTC if necessary
                            dateRangeQuery.Lte = DateTime.SpecifyKind(endMonth, DateTimeKind.Local).ToUniversalTime();

                            mustQueries.Add(dateRangeQuery);

                            var query = new BoolQuery { Must = mustQueries };

                            var countRequest = new CountRequest(requestModel.Index) { Query = query };

                            var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                            var sumResponse = await GetSumSearchResponse<LogRecord>(requestModel.Index, query, "elapsed_sum", "fields.ElapsedMilliseconds");


                            double elapsedSum = GetValueAggregationField<LogRecord>(sumResponse, "elapsed_sum");

                            double averageElapsed = CalculateAverage(elapsedSum, countResponse.Count);


                            var dataDetail = new DataDetail
                            {
                                AverageProcessedTime = Math.Round(averageElapsed, 2),
                                TotalResponse = countResponse.Count
                            };

                            statistics.Add(month.ToString(), dataDetail);

                            count += countResponse.Count;
                        }
                        break;

                    case "month":
                        var days = TimeExtension.GetDaysInMonth(date.Year, date.Month);
                        foreach (var day in days)
                        {
                            // Parse the input string into a DateTime object
                            DateTime dateTime = DateTime.ParseExact(day, "dd/MM", CultureInfo.InvariantCulture);

                            // Start time is at midnight (00:00:00)
                            DateTime startDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

                            // End time is at the last moment of the day (23:59:59)
                            DateTime endDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

                            // Convert to UTC if necessary
                            dateRangeQuery.Gte = DateTime.SpecifyKind(startDay, DateTimeKind.Local).ToUniversalTime();

                            // Convert to UTC if necessary
                            dateRangeQuery.Lte = DateTime.SpecifyKind(endDay, DateTimeKind.Local).ToUniversalTime();

                            mustQueries.Add(dateRangeQuery);

                            var query = new BoolQuery { Must = mustQueries };

                            var countRequest = new CountRequest(requestModel.Index) { Query = query };

                            var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                            var sumResponse = await GetSumSearchResponse<LogRecord>(requestModel.Index, query, "elapsed_sum", "fields.ElapsedMilliseconds");


                            double elapsedSum = GetValueAggregationField<LogRecord>(sumResponse, "elapsed_sum");


                            double averageElapsed = CalculateAverage(elapsedSum, countResponse.Count);


                            var dataDetail = new DataDetail
                            {
                                AverageProcessedTime = Math.Round(averageElapsed, 2),
                                TotalResponse = countResponse.Count
                            };

                            statistics.Add(day.ToString(), dataDetail);

                            count += countResponse.Count;
                        }
                        break;

                    case "day":
                        var hours = TimeExtension.GetHoursInday(date);
                        foreach (var hour in hours)
                        {
                            // Start time is at midnight (00:00:00)
                            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);

                            // End time is at the last moment of the day (23:59:59)
                            DateTime endTime = new DateTime(date.Year, date.Month, date.Day, hour, 59, 59);

                            // Convert to UTC if necessary
                            dateRangeQuery.Gte = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();

                            // Convert to UTC if necessary
                            dateRangeQuery.Lte = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                            mustQueries.Add(dateRangeQuery);

                            var query = new BoolQuery { Must = mustQueries };

                            var countRequest = new CountRequest(requestModel.Index) { Query = query };

                            var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                            var sumResponse = await GetSumSearchResponse<LogRecord>(requestModel.Index, query, "elapsed_sum", "fields.ElapsedMilliseconds");


                            double elapsedSum = GetValueAggregationField<LogRecord>(sumResponse, "elapsed_sum");


                            double averageElapsed = CalculateAverage(elapsedSum, countResponse.Count);


                            var dataDetail = new DataDetail
                            {
                                AverageProcessedTime = Math.Round(averageElapsed, 2),
                                TotalResponse = countResponse.Count
                            };

                            statistics.Add(hour.ToString(), dataDetail);

                            count += countResponse.Count;
                        }
                        break;

                    default:
                        throw new ArgumentException($"Invalid filter type: {requestModel.FilterType}");
                }

                return new StatisticLogDetailResponse
                {
                    Data = statistics,
                    Total = count,
                };
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetUserLogResponseDetailStatistic catch {exceptionError}");
            }
        }

        private async Task<SearchResponse<T>> GetSumSearchResponse<T>(string index, BoolQuery query, string sumName, string field)
        {
            try
            {
                var sumResponse = await _elasticSearchClient.SearchAsync<T>(s => s
                                            .Index(index)
                                            .Size(0)
                                            .Query(query)
                                            .Aggregations(aggs => aggs
                                                .Add(sumName, agg => agg
                                                    .Sum(tt => tt
                                                          .Field(field)
                                                    )

                                                )
                                            )

                                            );
                return sumResponse;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetSumSearchResponse catch {exceptionError}");
            }
        }

        private double CalculateAverage(double total, long numberOfRecord)
        {
            return numberOfRecord != 0 ? total / numberOfRecord : 0;
        }

        private double GetValueAggregationField<T>(SearchResponse<T> searchResponse, string key)
        {
            try
            {
                // Retrieve the value of the "elapsed_sum" aggregation
                if (searchResponse.Aggregations.TryGetValue(key, out var keySumAgg))
                {
                    return (double)((keySumAgg as SumAggregate)?.Value);
                }
                return 0;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);

                _logger.LogError("GetValueAggregationField catch exception: {Exception}", exceptionError);
                throw;

            }
        }

        public async Task<StatisticLogDetailResponse> GetServiceUsageStatistic(FilterServiceUsagelRequestModel requestModel)
        {
            try
            {
                long count = 0;

                // Build the query
                var mustQueries = new List<Query>();

                mustQueries.Add(new TermQuery(field: "fields.LogType.raw")
                {
                    Value = "Response"
                });



                mustQueries.Add(new TermQuery(field: "fields.Environment.raw")
                {
                    Value = requestModel.Environment
                });

                if (!string.IsNullOrEmpty(requestModel.RequestPath) && !string.IsNullOrEmpty(requestModel.Method))
                {
                    mustQueries.Add(new TermQuery(field: "fields.RequestPath.raw")
                    {
                        Value = requestModel.RequestPath
                    });

                    mustQueries.Add(new TermQuery(field: "fields.Method.raw")
                    {
                        Value = requestModel.Method.ToUpper()
                    });
                }

                if (!string.IsNullOrEmpty(requestModel.StatusCode))
                {
                    var statusCodeRangeQuery = new NumberRangeQuery(field: "fields.StatusCode");


                    switch (requestModel.StatusCode)
                    {
                        case "1XX":

                            statusCodeRangeQuery.Gte = 100;
                            statusCodeRangeQuery.Lte = 199;
                            break;

                        case "2XX":

                            statusCodeRangeQuery.Gte = 200;
                            statusCodeRangeQuery.Lte = 299;
                            break;

                        case "3XX":

                            statusCodeRangeQuery.Gte = 300;
                            statusCodeRangeQuery.Lte = 399;
                            break;

                        case "4XX":

                            statusCodeRangeQuery.Gte = 400;
                            statusCodeRangeQuery.Lte = 499;
                            break;

                        case "5XX":

                            statusCodeRangeQuery.Gte = 500;
                            statusCodeRangeQuery.Lte = 599;
                            break;
                    }


                    mustQueries.Add(statusCodeRangeQuery);
                }

                var dateRangeQuery = new DateRangeQuery(field: "@timestamp");



                var format = "dd-MM-yyyy HH:mm:ss";
                var culture = CultureInfo.InvariantCulture;

                DateTime.TryParseExact(requestModel.FromDate, format, culture, DateTimeStyles.None, out var fromDate);
                DateTime.TryParseExact(requestModel.ToDate, format, culture, DateTimeStyles.None, out var toDate);

                // Start time is at midnight (00:00:00)
                DateTime startTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);

                // End time is at the last moment of the day (23:59:59)
                DateTime endTime = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

                // Convert to UTC if necessary
                dateRangeQuery.Gte = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();

                // Convert to UTC if necessary
                dateRangeQuery.Lte = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                mustQueries.Add(dateRangeQuery);

                Dictionary<string, DataDetail> statistics = new Dictionary<string, DataDetail>();

                var usersCache = await _cachedEndpointUserRepository.GetEndpointUser(requestModel.RequestPath, requestModel.Method);

                if (usersCache is null)
                {
                    var users = await _identityGrpcService.GetUserInServiceEndpoint(requestModel.RequestPath, requestModel.Method);

                    if (users.Any())
                    {
                        var endpointUser = new EndpointUser
                        {
                            Method = requestModel.Method,
                            Url = requestModel.RequestPath,
                            Users = users.Select(u => new User
                            {
                                Id = u.Id,
                                Email = u.Email
                            }),
                        };

                        usersCache = users.Select(u => new User
                        {
                            Email = u.Email,
                            Id = u.Id,
                        });

                        await _cachedEndpointUserRepository.AddEndpointUser(endpointUser);
                    }
                    else
                    {
                        throw new LogInternalException("Users not found in service");
                    }
                }


                foreach (var user in usersCache)
                {
                    mustQueries.Add(new TermQuery(field: "fields.UserId")
                    {
                        Value = user.Id
                    });


                    var query = new BoolQuery { Must = mustQueries };

                    var countRequest = new CountRequest(requestModel.Index) { Query = query };

                    var countResponse = await _elasticSearchClient.CountAsync(countRequest);

                    var sumResponse = await GetSumSearchResponse<LogRecord>(requestModel.Index, query, "elapsed_sum", "fields.ElapsedMilliseconds");


                    double elapsedSum = GetValueAggregationField<LogRecord>(sumResponse, "elapsed_sum");


                    double averageElapsed = CalculateAverage(elapsedSum, countResponse.Count);


                    var dataDetail = new DataDetail
                    {
                        AverageProcessedTime = Math.Round(averageElapsed, 2),
                        TotalResponse = countResponse.Count
                    };

                    statistics.Add(user.Email, dataDetail);

                    count += countResponse.Count;

                    mustQueries.RemoveAt(mustQueries.Count() - 1);
                }

                return new StatisticLogDetailResponse
                {
                    Data = statistics,
                    Total = count,
                };
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetServiceUsageStatistic catch {exceptionError}");
            }
        }

        public async Task<PaginatedResponse<LogRecord>> GetLogsPaging(LogSearchRequest request)
        {
            try
            {
                var mustQueries = new List<Elastic.Clients.Elasticsearch.QueryDsl.Query>();
                var mustNotQueries = new List<Query>();

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    mustQueries.Add(new SimpleQueryStringQuery
                    {
                        Query = request.SearchTerm + "*"
                    });
                }

                // Date range filter
                if (!string.IsNullOrEmpty(request.StartDate) || !string.IsNullOrEmpty(request.EndDate))
                {
                    var dateRangeQuery = new DateRangeQuery(field: "@timestamp");

                    // Adjusted format string for parsing
                    var format = "dd-MM-yyyy HH:mm:ss";
                    var culture = CultureInfo.InvariantCulture;

                    if (DateTime.TryParseExact(request.StartDate, format, culture, DateTimeStyles.None, out var startDate))
                    {
                        // Convert to UTC if necessary
                        dateRangeQuery.Gte = DateTime.SpecifyKind(startDate, DateTimeKind.Local).ToUniversalTime();
                    }
                    if (DateTime.TryParseExact(request.EndDate, format, culture, DateTimeStyles.None, out var endDate))
                    {
                        // Convert to UTC if necessary
                        dateRangeQuery.Lte = DateTime.SpecifyKind(endDate, DateTimeKind.Local).ToUniversalTime();
                    }

                    mustQueries.Add(dateRangeQuery);
                }

                // Xử lý Filters
                if (request.Filters != null && request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        switch (filter.Operator.ToLower())
                        {
                            case "exists":
                                mustQueries.Add(new ExistsQuery
                                {
                                    Field = filter.Field
                                });
                                break;

                            case "not_exists":
                                mustNotQueries.Add(new ExistsQuery
                                {
                                    Field = filter.Field
                                });
                                break;

                            case "equals":
                                mustQueries.Add(new TermQuery(field: filter.Field)
                                {
                                    Value = filter.Value
                                });
                                break;

                            case "not_equals":
                                // Ensure the field exists
                                mustQueries.Add(new ExistsQuery
                                {
                                    Field = filter.Field
                                });

                                // Exclude documents where the field equals the specified value
                                mustNotQueries.Add(new TermQuery(field: filter.Field)
                                {
                                    Value = filter.Value
                                });
                                break;

                            case "contains":
                                mustQueries.Add(new MatchQuery(field: filter.Field)
                                {
                                    Query = filter.Value
                                });
                                break;

                            case "not_contains":
                                // Ensure the field exists
                                mustQueries.Add(new ExistsQuery
                                {
                                    Field = filter.Field
                                });

                                // Exclude documents where the field matches the specified value
                                mustNotQueries.Add(new MatchQuery(field: filter.Field)
                                {
                                    Query = filter.Value
                                });
                                break;

                            default:
                                throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.");
                        }
                    }
                }


                var sortOrder = SortOrder.Desc;

                if (request.IsLatest)
                {
                    sortOrder = SortOrder.Asc;
                }
                var query = new BoolQuery { Must = mustQueries, MustNot = mustNotQueries };
                // Execute the search
                var response = await _elasticSearchClient.SearchAsync<LogRecord>(s => s
                                            .Index(request.Index)
                                            .Query(query)
                                            .Sort(r => r.Field("@timestamp", new FieldSort
                                            {
                                                Order = sortOrder
                                            }))
                                            .From(request.PageIndex * request.PageSize)
                                            .Size(request.PageSize)
                                            .SourceIncludes(new[]
                                                                 {
                                                                 "@timestamp",
                                                                 "fields.CorrelationId",
                                                                 "fields.RequestPath",
                                                                 "level",
                                                                 "message",
                                                                 "fields.Method",
                                                                 "fields.StatusCode"
                                                                  })
                                            );

                var logRecords = response.Hits.Select(hit => new LogRecord
                {
                    Id = hit.Id, // Extract _id from the hit metadata
                    Timestamp = hit.Source.Timestamp,
                    Level = hit.Source.Level,
                    Message = hit.Source.Message,
                    Fields = hit.Source.Fields
                }).ToList();


                if (!response.IsValidResponse)
                {
                    throw new LogInternalException("Elasticsearch query failed.");
                }

                var countResponse = await _elasticSearchClient.CountAsync<LogRecord>(c => c.Indices(request.Index).Query(query));

                var totalCount = countResponse.Count;

                var result = new PaginatedResponse<LogRecord>
                {
                    Items = logRecords,
                    PageIndex = (int)request.PageIndex,
                    PageSize = (int)request.PageSize,
                    TotalCount = (int)totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                    HasPreviousPage = request.PageIndex > 0,
                    HasNextPage = request.PageIndex < (int)Math.Ceiling(totalCount / (double)request.PageSize) - 1
                };

                return result;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);
                throw new LogInternalException($"GetLogsPaging catch {exceptionError}");
            }
        }
    }
}