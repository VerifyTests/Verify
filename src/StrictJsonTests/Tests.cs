[UsesVerify]
public class Tests
{
    [Fact]
    public Task WriteRawInConverterTest()
    {
        var target = new WriteRawInConverterTarget();
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new WriteRawInConverter())).ScrubEmptyLines();
    }

    class WriteRawInConverterTarget
    {
    }

    class WriteRawInConverter:
        WriteOnlyJsonConverter<WriteRawInConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, WriteRawInConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Raw");
            writer.WriteRawValue("Raw \" value");
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
        Verify(new {value = "Foo"});

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
    public Task WithInfo()
    {
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
        return Verify(new MemoryStream(new byte[]{1}))
            .UseExtension("foo");
    }

    [Fact]
    public void DuplicateSerializationTests()
    {
        var projectDirectory = AttributeReader.GetProjectDirectory();
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        File.Copy(
            Path.Combine(solutionDirectory, @"Verify.Tests\Serialization\SerializationTests.cs"),
            Path.Combine(projectDirectory, @"Serialization\SerializationTests.cs"),
            true);
    }
}

public class TheTarget
{
    public string? Value { get; set; }
}