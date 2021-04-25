#if NET5_0
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                if (request.Content.Headers.Any())
                {
                    RequestContentHeaders = request.Content.Headers;
                }

                var requestStringContent = TryReadStringContent(request.Content);
                RequestContentString = requestStringContent.prettyContent;
                RequestContentStringRaw = requestStringContent.content;
            }

            ResponseHeaders = response.Headers;
            if (response.Content.Headers.Any())
            {
                ResponseContentHeaders = response.Content.Headers;
            }
            var responseStringContent = TryReadStringContent(response.Content);
            ResponseContentString = responseStringContent.prettyContent;
            ResponseContentStringRaw = responseStringContent.content;

            if (status != TaskStatus.RanToCompletion)
            {
                Status = status;
            }

            Duration = Activity.Current!.Duration;
        }

        static (string? content, string? prettyContent) TryReadStringContent(HttpContent content)
        {
            if (!content.IsText(out var subType))
            {
                return (null, null);
            }

            var stringContent = content.ReadAsStringAsync().GetAwaiter().GetResult();
            var prettyContent = stringContent;
            if (subType == "json")
            {
                try
                {
                    prettyContent = JToken.Parse(stringContent).ToString();
                }
                catch
                {
                }
            }
            else if (subType == "xml")
            {
                try
                {
                    prettyContent = XDocument.Parse(stringContent).ToString();
                }
                catch
                {
                }
            }

            return (stringContent, prettyContent);
        }

        [JsonIgnore]
        public TimeSpan Duration { get; }

        public Uri Uri { get; }

        public TaskStatus? Status { get; }

        public HttpRequestHeaders? RequestHeaders { get; }
        public HttpContentHeaders? RequestContentHeaders { get; }
        public string? RequestContentString { get; }
        [JsonIgnore]
        public string? RequestContentStringRaw { get; }

        public HttpResponseHeaders ResponseHeaders { get; }
        public HttpContentHeaders? ResponseContentHeaders { get; }
        public string? ResponseContentString { get; }
        [JsonIgnore]
        public string? ResponseContentStringRaw { get; }
    }
}
#endif