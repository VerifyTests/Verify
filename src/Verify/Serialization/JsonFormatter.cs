using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VerifyTests;

static class JsonFormatter
{
    internal static char QuoteChar = '\'';

    public static StringBuilder AsJson(object? input, JsonSerializerSettings settings, bool newLineEscapingDisabled, List<ToAppend> appends)
    {
        if (appends.Any())
        {
            var dictionary = new Dictionary<string, object>();
            if (input == null)
            {
                dictionary.Add("target", "null");
            }
            else
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
        var builder = new StringBuilder();
        using var stringWriter = new StringWriterEx(builder, newLineEscapingDisabled);
        using var writer = new JsonTextWriterEx(stringWriter)
        {
            QuoteChar = QuoteChar,
            QuoteName = false
        };
        serializer.Serialize(writer, input);
        return builder;
    }
}