static class JsonFormatter
{
    static JsonFormatter()
    {
        TypeNameConverter.AddRedirect(typeof(IDictionaryWrapper), _ => _.GetGenericArguments().Last());
    }

    public static StringBuilder AsJson(object? input, List<ToAppend> appends, VerifySettings settings, Counter counter)
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

        var builder = new StringBuilder();
        using var writer = new VerifyJsonWriter(builder, settings.serialization, settings.Context, counter);
        settings.Serializer.Serialize(writer, input);
        return builder;
    }
}