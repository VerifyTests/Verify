namespace VerifyTests;

public abstract partial class WriteOnlyJsonConverter
{
    public static string Execute<TConverter>(
        object target,
        VerifySettings? settings = null)
        where TConverter : WriteOnlyJsonConverter, new() =>
        Execute(new TConverter(), target, settings);

    public static string Execute<TConverter>(TConverter converter, object target, VerifySettings? settings = null)
        where TConverter : WriteOnlyJsonConverter
    {
        settings ??= new();
        settings.UseStrictJson();
        var builder = new StringBuilder("{");
        using var counter = Counter.Start();
        using (var writer = new VerifyJsonWriter(builder, settings, counter))
        {
            converter.Write(writer, target);
        }

        builder.Append('}');
        return builder.ToString();
    }
}