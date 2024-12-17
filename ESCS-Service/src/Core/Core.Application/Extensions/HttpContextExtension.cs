using Core.Application.Common;
using Core.Application.Exceptions;
using Newtonsoft.Json;
using System.Text;

namespace Core.Application.Extensions
{
    public static class HttpContextExtension
    {



        public static HttpRequestMessage CloneHttpRequestMessage(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content != null ? new StringContent(request.Content.ReadAsStringAsync().Result) : null
            };

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var header in request.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
            {
                clone.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }


        // Main method to handle both GET and POST requests
        public static HttpRequestMessage CreateHttpRequestMessage<TRequest>(HttpMethod method, HttpCallOptions<TRequest> httpCallOptions)
        {
            var requestMessage = CreateBaseHttpRequestMessage(method, httpCallOptions.BaseUrl, httpCallOptions.QueryStringElements);

            // Attach the body for non-GET requests if it exists
            if (method != HttpMethod.Get && httpCallOptions.Body != null)
            {
                requestMessage.Content = GetContent(httpCallOptions.Body);
            }

            // Attach authentication and standard headers
            AddAuthenticationAndHeaders(requestMessage, httpCallOptions.AuthenticationType, httpCallOptions.ApiKey);

            return requestMessage;
        }

        // Overloaded version for GET requests that don't require a generic body
        public static HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, HttpCallOptions httpCallOptions)
        {
            var requestMessage = CreateBaseHttpRequestMessage(method, httpCallOptions.BaseUrl, httpCallOptions.QueryStringElements);

            // Attach authentication and standard headers
            AddAuthenticationAndHeaders(requestMessage, httpCallOptions.AuthenticationType, httpCallOptions.ApiKey);

            return requestMessage;
        }

        // Helper method to create the base HttpRequestMessage with common logic
        private static HttpRequestMessage CreateBaseHttpRequestMessage(HttpMethod method, string baseUrl, List<string>? queryStringElements = null)
        {
            // Handle query string elements
            var requestUri = queryStringElements != null && queryStringElements.Count > 0
                ? CreateQueryString(baseUrl, queryStringElements)
                : baseUrl;

            return new HttpRequestMessage(method, requestUri);
        }

        // Helper method to attach body content
        private static HttpContent GetContent(object body)
        {
            return body is MultipartFormDataContent multipartContent
                ? multipartContent
                : GetBody(body); // Assume this method handles JSON serialization
        }

        // Helper method to add authentication and standard headers
        private static void AddAuthenticationAndHeaders(HttpRequestMessage requestMessage, string? authType, string? apiKey)
        {
            // Attach authentication headers
            if (!string.IsNullOrEmpty(authType) && !string.IsNullOrEmpty(apiKey))
            {
                requestMessage.Headers.Remove(authType);
                requestMessage.Headers.Add(authType, apiKey);
            }

            // Standard headers
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "HttpClientFactory-Sample");
        }



        private static string CreateQueryString(string baseUrl, List<string> queryStringList)
        {
            var queryStringBuilder = new StringBuilder();

            queryStringBuilder.Append(baseUrl);

            for (int index = 0; index < queryStringList.Count; index++)
            {
                if (index == 0)
                    queryStringBuilder.Append("?");

                queryStringBuilder.Append(queryStringList[index]);

                if (index != queryStringList.Count - 1)
                    queryStringBuilder.Append("&");
            }

            return queryStringBuilder.ToString();
        }

        //Serialize request body
        private static StringContent? GetBody(object? data)
        {
            try
            {
                if (data == null) return null;
                var body = JsonConvert.SerializeObject(data);
                return new StringContent(body, Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {

                throw new BusinessException(ex.Message);
            }
        }
    }
}
