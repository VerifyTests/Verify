using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VerifyTests;

static class JsonFormatter
{
    internal static char QuoteChar = '\'';

    public static StringBuilder AsJson(object? input, JsonSerializerSettings settings, List<ToAppend> appends, VerifySettings verifySettings)
    {
        if (appends.Any())
        {
            Dictionary<string, object> dictionary = new();
            if (input != null)
            {
                dictionary.Add("target", input);
            }

            input = dictionary;
            foreach (var append in appends)
            {
                dictionary[append.Name] = append.Data;
            }
        }

        var serializer = JsonSerializer.Create(settings);

        StringBuilder builder = new();
        using StringWriter stringWriter = new(builder)
        {
            NewLine = "\n"
        };
        using JsonTextWriterEx writer = new(stringWriter, verifySettings.Context)
        {
            QuoteChar = QuoteChar,
            QuoteName = false
        };
        serializer.Serialize(writer, input);
        return builder;
    }
}