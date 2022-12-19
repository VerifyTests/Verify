[UsesVerify]
public class StaticFileAppenderWithScrubberTests :
    IDisposable
{
    static AsyncLocal<bool> isInThisTest = new();

    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.RegisterFileAppender(
            _ =>
            {
                if (!isInThisTest.Value)
                {
                    return null;
                }

                return new("txt", "data");
            });
        VerifierSettings.AddScrubber(
            "txt", _ =>
            {
                if (!isInThisTest.Value)
                {
                    return;
                }

                _.Append("Appended");
            });
    }

    public StaticFileAppenderWithScrubberTests() =>
        isInThisTest.Value = true;

    public void Dispose() =>
        isInThisTest.Value = false;

    [Fact]
    public Task Text() =>
        Verify("Foo");
}