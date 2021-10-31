using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

static class JsonFormatter
{
    static JsonFormatter()
    {
        TypeNameConverter.AddRedirect(typeof(IDictionaryWrapper), _ => _.GetGenericArguments().Last());
    }

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

        using var writer = new JsonTextWriterEx(stringWriter, builder, verifySettings.Context);
        serializer.Serialize(writer, input);
        return builder;
    }

}