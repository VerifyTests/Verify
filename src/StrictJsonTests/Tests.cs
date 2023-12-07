[UsesVerify]
public class Tests
{
    [Fact]
    public Task WriteRawInConverterTest()
    {
        var target = new WriteRawInConverterTarget();
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new WriteRawInConverter()))
            .ScrubEmptyLines();
    }

    class WriteRawInConverterTarget;

    class WriteRawInConverter :
        WriteOnlyJsonConverter<WriteRawInConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, WriteRawInConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Raw");
            writer.WriteRawValueIfNoStrict("Raw \" value");
            writer.WritePropertyName("WriteValue");
            writer.WriteValue("Write \" Value");
            writer.WritePropertyName("WriteRawWithScrubbers");
            writer.WriteRawValueWithScrubbers("Write \"\r\r\rRawWithScrubbers\r\r");
            writer.WriteEndObject();
        }
    }

    [Fact]
    public Task String() =>
        Verify("Foo");

    [Fact]
    public void ValidateJson()
    {
        foreach (var file in Directory.EnumerateFiles(AttributeReader.GetProjectDirectory(), "*.verified.json"))
        {
            try
            {
                var readAllText = File.ReadAllText(file);
                if (readAllText.StartsWith('['))
                {
                    JArray.Parse(readAllText);
                }
                else
                {
                    JObject.Parse(readAllText);
                }
            }
            catch (Exception exception)
            {
                throw new(file, exception);
            }
        }
    }

    [Fact]
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonArrayString()
    {
        var json = "[{'key': {'msg': 'This is a proper json string'}}, {'key': {'msg': 'Foo'}}]";
        return VerifyJson(json);
    }

    [Fact]
    public Task Dynamic() =>
        Verify(new
        {
            value = "Foo"
        });

    [Fact]
    public async Task Object()
    {
        #region UseStrictJsonVerify

        var target = new TheTarget
        {
            Value = "Foo"
        };
        await Verify(target);

        #endregion
    }

    [Fact]
    public Task WithInfo() =>
        Verify(new MemoryStream([1]), "foo");

    [ModuleInitializer]
    public static void WithInfoInit() =>
        VerifierSettings.RegisterFileConverter(
            "foo",
            (_, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "txt", "content");
            });
}

public class TheTarget
{
    public string? Value { get; set; }
}