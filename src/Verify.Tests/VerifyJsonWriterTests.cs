public class VerifyJsonWriterTests
{
    [Fact]
    public void ShouldWriteScrubbed_WhenValueIsIgnored()
    {
        var builder = new StringBuilder();
        var settings = new VerifySettings();
        settings.IgnoreMember("name");
        var writer = new VerifyJsonWriter(builder, settings, CounterBuilder.Empty());

        writer.WriteMember(new(), "value".AsSpan(), "name");

        Assert.DoesNotContain("name", builder.ToString());
    }

    [Fact]
    public void ShouldWriteScrubbed_WhenValueIsScrubbed()
    {
        var builder = new StringBuilder();
        var settings = new VerifySettings();
        settings.ScrubMember("name");
        var writer = new VerifyJsonWriter(builder, settings, CounterBuilder.Empty());

        writer.WriteMember(new(), "value".AsSpan(), "name");

        Assert.Contains("name: Scrubbed", builder.ToString());
    }

    [Fact]
    public void ShouldWriteValue()
    {
        var builder = new StringBuilder();
        var settings = new VerifySettings
        {
            serialization = new()
        };
        var writer = new VerifyJsonWriter(builder, settings, CounterBuilder.Empty());

        writer.WriteMember(new(), "value".AsSpan(), "name");

        Assert.Contains("name: value", builder.ToString());
    }
}