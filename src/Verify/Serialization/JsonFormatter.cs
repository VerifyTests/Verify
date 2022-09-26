static class JsonFormatter
{
    public static StringBuilder AsJson(object? input, List<ToAppend> appends, VerifySettings settings, Counter counter)
    {
        var infoBuilder = new InfoBuilder(input);
        foreach (var append in appends)
        {
            infoBuilder.Add(append.Name, append.Data);
        }

        var builder = new StringBuilder();
        using var writer = new VerifyJsonWriter(builder, settings, counter);
        settings.Serializer.Serialize(writer, infoBuilder);
        builder.FixNewlines();
        return builder;
    }
}