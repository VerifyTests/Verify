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
        HttpHeaders headers,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        var value = headers
            .OrderBy(x => x.Key.ToLowerInvariant())
            .ToDictionary(
                x => x.Key,
                x =>
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