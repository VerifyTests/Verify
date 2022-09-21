﻿static class JsonFormatter
{
    public static StringBuilder AsJson(object? input, List<ToAppend> appends, VerifySettings settings, Counter counter)
    {
        if (appends.Any())
        {
            var dictionary = new MultiValueDictionary();
            dictionary.Add("target", input ?? "null");

            input = dictionary;
            foreach (var append in appends)
            {
                dictionary.Add(append.Name, append.Data);
            }
        }

        var builder = new StringBuilder();
        using var writer = new VerifyJsonWriter(builder, settings, counter);
        settings.Serializer.Serialize(writer, input);
        builder.FixNewlines();
        return builder;
    }
}