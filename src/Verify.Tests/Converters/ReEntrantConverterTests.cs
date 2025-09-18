public class ReEntrantConverterTests
{
    [ModuleInitializer]
    public static void InheritedInit()
    {
        VerifierSettings.RegisterStreamConverter("reentrant", (_, target, settings) => Convert(target, settings));
        VerifierSettings.RegisterFileConverter<Target>(Convert);
    }

    static ConversionResult Convert(Stream target, IReadOnlyDictionary<string, object> settings)
    {
        var reader = new StreamReader(target);
        var readToEnd = reader.ReadToEnd();
        return Convert(new Target {Property = readToEnd}, settings);
    }

    static ConversionResult Convert(Target instance, IReadOnlyDictionary<string, object> settings)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(instance.Property);
        writer.Flush();
        stream.Position = 0;
        VerifyTests.Target[] targets =
        [
            new("reentrant", stream),
            new("txt", instance.Property)
        ];
        return new(null, targets);
    }

    [Fact]
    public Task Test()
    {
        var target = new Target
        {
            Property = "Value"
        };
        return Verify(target);
    }

    class Target
    {
        public required string Property { get; init; }
    }
}
