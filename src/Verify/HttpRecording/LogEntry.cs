using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VerifyTests
{
    public class LogEntry
    {
        public LogEntry(HttpRequestMessage request, HttpResponseMessage response, TaskStatus status)
        {
            Uri = request.RequestUri!;
            RequestHeaders = request.Headers;
            ResponseHeaders = response.Headers;
            Status = status;
            Duration = Activity.Current!.Duration;
        }

        [JsonIgnore]
        public TimeSpan Duration { get; }

        public Uri Uri { get; }

        public TaskStatus Status { get; }

        public HttpRequestHeaders RequestHeaders { get; }

        public HttpResponseHeaders ResponseHeaders { get; }
    }
}