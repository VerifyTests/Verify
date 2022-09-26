static class JsonFormatter
{
    public static StringBuilder AsJson(VerifySettings settings, Counter counter, object value)
    {
        var builder = new StringBuilder();
        using var writer = new VerifyJsonWriter(builder, settings, counter);
        settings.Serializer.Serialize(writer, value);
        builder.FixNewlines();
        return builder;
    }
}