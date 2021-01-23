using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using VerifyTests;

class HttpHeadersConverter :
    WriteOnlyJsonConverter<HttpHeaders>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpHeaders? headers,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (headers == null)
        {
            return;
        }

        var value = headers
            .OrderBy(x => x.Key.ToLowerInvariant())
            .Where(x =>
            {
                var key = x.Key.ToLowerInvariant();
                return key != "traceparent" &&
                       key != "set-cookie";
            })
            .ToDictionary(x => x.Key, x =>
            {
                var values = x.Value.ToList();
                var key = x.Key.ToLowerInvariant();
                if (key == "date" ||
                    key == "expires" ||
                    key == "last-modified")
                {
                    if (DateTime.TryParse(values.First(), out var date))
                    {
                        return date;
                    }
                }

                return (object) string.Join(",", values);
            });
        serializer.Serialize(writer, value);
    }
}