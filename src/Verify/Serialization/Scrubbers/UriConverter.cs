using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using VerifyTests;

class UriConverter :
    WriteOnlyJsonConverter<Uri>
{
    public override void WriteJson(JsonWriter writer, Uri? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value.Query))
        {
            writer.WriteValue(value.OriginalString);
            return;
        }

        string path;
        var scheme = value.Scheme;
        var port = value.Port;
        if (scheme == "http" && port == 80)
        {
            path = $"http://{value.Host}{value.AbsolutePath}";
        }
        else if (scheme == "https" && port == 443)
        {
            path = $"https://{value.Host}{value.AbsolutePath}";
        }
        else
        {
            path = $"{scheme}://{value.Host}:{port}{value.AbsolutePath}";
        }

        serializer.Serialize(writer,
            new UriWrapper
            {
                Path = path,
                Query = ParseQueryString(value.Query)
            });
    }

    class UriWrapper
    {
        public string Path { get; set; } = null!;
        public Dictionary<string, string?> Query { get; set; }= null!;
    }

    static Dictionary<string, string?> ParseQueryString(string queryString)
    {
        var dictionary = new Dictionary<string,string?>();
        var collection = HttpUtility.ParseQueryString(queryString);
        foreach (string key in collection)
        {
            dictionary[key] = collection[key];
        }

        return dictionary;
    }
}