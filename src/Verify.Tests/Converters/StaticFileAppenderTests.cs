[UsesVerify]
public class StaticFileAppenderTests :
    IDisposable
{
    static AsyncLocal<bool> isInThisTest = new();

    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.RegisterFileAppender(
            _ =>
            {
                if (!isInThisTest.Value)
                {
                    return null;
                }

                return new("txt", "data");
            });

    public StaticFileAppenderTests() =>
        isInThisTest.Value = true;

    public void Dispose() =>
        isInThisTest.Value = false;

    [Fact]
    public Task Text() =>
        Verify("Foo");

    [Fact]
    public Task EmptyString() =>
        Verify(string.Empty);

    [Fact]
    public Task Anon() =>
        Verify(new
        {
            foo = "bar"
        });

    [Fact]
    public Task NullText() =>
        Verify((string?) null);

    [Fact]
    public Task Stream() =>
        Verify(IoHelpers.OpenRead("sample.txt"));

    [Fact]
    public Task File() =>
        VerifyFile("sample.txt");
}