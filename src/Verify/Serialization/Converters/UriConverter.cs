using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using VerifyTests;

class UriConverter :
    WriteOnlyJsonConverter<Uri>
{
    public override void WriteJson(
        JsonWriter writer,
        Uri value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (string.IsNullOrWhiteSpace(value.Query))
        {
            writer.WriteValue(value.OriginalString);
            return;
        }

        var path = GetPath(value);

        serializer.Serialize(writer,
            new UriWrapper
            {
                Path = path,
                Query = HttpUtility.ParseQueryString(value.Query)
            });
    }

    static string GetPath(Uri value)
    {
        var scheme = value.Scheme;
        var port = value.Port;
        if (scheme == "http" && port == 80)
        {
            return $"http://{value.Host}{value.AbsolutePath}";
        }

        if (scheme == "https" && port == 443)
        {
            return $"https://{value.Host}{value.AbsolutePath}";
        }

        return $"{scheme}://{value.Host}:{port}{value.AbsolutePath}";
    }

    class UriWrapper
    {
        public string Path { get; set; } = null!;
        public NameValueCollection Query { get; set; }= null!;
    }
}