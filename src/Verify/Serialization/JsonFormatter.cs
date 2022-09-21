static class JsonFormatter
{
    public static StringBuilder AsJson(object? input, List<ToAppend> appends, VerifySettings settings, Counter counter)
    {
        if (appends.Any())
        {
            var infoBuilder = new InfoBuilder();
            infoBuilder.Add("target", input ?? "null");

            input = infoBuilder;
            foreach (var append in appends)
            {
                infoBuilder.Add(append.Name, append.Data);
            }
        }

        var builder = new StringBuilder();
        using var writer = new VerifyJsonWriter(builder, settings, counter);
        settings.Serializer.Serialize(writer, input);
        builder.FixNewlines();
        return builder;
    }
}