using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VerifyTests;

static class JsonFormatter
{
    public static StringBuilder AsJson(object? input, JsonSerializerSettings settings, List<ToAppend> appends, VerifySettings verifySettings)
    {
        if (appends.Any())
        {
            var dictionary = new DictionaryWrapper<string, object>();
            if (input is null)
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
        using StringWriter stringWriter = new(builder)
        {
            NewLine = "\n"
        };

        using JsonTextWriterEx writer = new(stringWriter, builder, verifySettings.Context);
        serializer.Serialize(writer, input);
        return builder;
    }

}