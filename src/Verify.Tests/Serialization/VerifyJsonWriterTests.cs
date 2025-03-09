public class VerifyJsonWriterTests
{
    [Fact]
    public void ShouldWriteScrubbed_WhenValueIsIgnored()
    {
        // Arrange
        var builder = new StringBuilder();
        var settings = new VerifySettings();
        settings.IgnoreMember("ignoredName");
        var writer = new VerifyJsonWriter(builder, settings, CounterBuilder.Empty());

        writer.WriteMember(new(), "test".AsSpan(), "ignoredName");

        Assert.DoesNotContain("ignoredName", builder.ToString());
    }

    [Fact]
    public void ShouldWriteScrubbed_WhenValueIsScrubbed()
    {
        var builder = new StringBuilder();
        var settings = new VerifySettings();
        settings.ScrubMember("ignoredName");
        var writer = new VerifyJsonWriter(builder, settings, CounterBuilder.Empty());

        writer.WriteMember(new(), "test".AsSpan(), "scrubbedName");

        Assert.Contains("\"scrubbedName\":\"Scrubbed\"", builder.ToString());
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

        writer.WriteMember(new(), "test".AsSpan(), "name");

        Assert.Contains("name:test", builder.ToString());
    }
}