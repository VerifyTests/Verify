#if NET9_0_OR_GREATER
public class TestingConverters
{
    record Target(string? Property);

    [Fact]
    public Task Test()
    {
        var converter = new Converter();
        var builder = new StringBuilder();

        var settings = new VerifySettings();
        using var counter = Counter.Start();

        var target = new Target("Value");

        using (var writer = new VerifyJsonWriter(builder, settings, counter))
        {
            converter.Write(writer, target);
        }

        return VerifyJson(builder);
    }

    class Converter :
        WriteOnlyJsonConverter<Target>
    {
        public override void Write(VerifyJsonWriter writer, Target target)
        {
            writer.WriteStartObject();
            writer.WriteMember(target, target.Property, "Property");
            writer.WritePropertyName("Extra");
            writer.Serialize(
                new Dictionary<string, string>
                {
                    {
                        "key", "value"
                    }
                });
            writer.WriteEnd();
        }
    }
}
#endif