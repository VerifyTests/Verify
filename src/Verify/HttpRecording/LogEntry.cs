using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VerifyTests
{
    public class LogEntry
    {
        public LogEntry(HttpRequestMessage request, HttpResponseMessage response, TaskStatus status)
        {
            RequestHeaders = request.Headers;
            ResponseHeaders = response.Headers;
            Status = status;
        }

        public TaskStatus Status { get; set; }

        public HttpResponseHeaders ResponseHeaders { get; set; }

        public HttpRequestHeaders RequestHeaders { get; }
    }
}