using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VerifyTests
{
    public class HttpCall
    {
        public HttpCall(HttpRequestMessage request, HttpResponseMessage response, TaskStatus status)
        {
            Uri = request.RequestUri!;
            if (request.Headers.Any())
            {
                RequestHeaders = request.Headers;
            }

            if (request.Content != null)
            {
                RequestContentHeaders = request.Content.Headers;
                RequestContentString = TryReadStringContent(request.Content);
            }

            ResponseHeaders = response.Headers;
            ResponseContentHeaders = response.Content.Headers;
            ResponseContentString = TryReadStringContent(response.Content);

            if (status != TaskStatus.RanToCompletion)
            {
                Status = status;
            }

            Duration = Activity.Current!.Duration;
        }

        string? TryReadStringContent(HttpContent content)
        {
            if (!IsStringContent(content))
            {
                return null;
            }

            return content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        static bool IsStringContent(HttpContent httpContent)
        {
            var type = httpContent.Headers.ContentType?.MediaType;
            if (type == null)
            {
                return false;
            }

            return type.StartsWith("text") ||
                   type.EndsWith("graphql") ||
                   type.EndsWith("javascript") ||
                   type.EndsWith("json") ||
                   type.EndsWith("xml");
        }

        [JsonIgnore]
        public TimeSpan Duration { get; }

        public Uri Uri { get; }

        public TaskStatus? Status { get; }

        public HttpRequestHeaders? RequestHeaders { get; }
        public HttpContentHeaders? RequestContentHeaders { get; }
        public string? RequestContentString { get; }

        public HttpResponseHeaders ResponseHeaders { get; }
        public HttpContentHeaders ResponseContentHeaders { get; }
        public string? ResponseContentString { get; }
    }
}