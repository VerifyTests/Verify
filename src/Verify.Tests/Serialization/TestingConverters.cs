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
        Counter.Start(
            dateCounting: false,
            scrubDateTimes: false,
            scrubGuids: false,
            namedDates: [],
            namedTimes: [],
            namedDateTimes: [],
            namedGuids: [],
            namedDateTimeOffsets: []
        );

        var target = new Target("Value");

        using (var writer = new VerifyJsonWriter(builder, settings, Counter.Current))
        {
            converter.Write(writer, target);
        }

        return Verify(builder);
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