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
            RequestContentHeaders = request.Content?.Headers;
            ResponseHeaders = response.Headers;
            ResponseContentHeaders = response.Content.Headers;
            if (status != TaskStatus.RanToCompletion)
            {
                Status = status;
            }
            Duration = Activity.Current!.Duration;
        }

        [JsonIgnore]
        public TimeSpan Duration { get; }

        public Uri Uri { get; }

        public TaskStatus? Status { get; }

        public HttpRequestHeaders? RequestHeaders { get; }
        public HttpContentHeaders? RequestContentHeaders { get; }

        public HttpResponseHeaders ResponseHeaders { get; }
        public HttpContentHeaders ResponseContentHeaders { get; }
    }
}