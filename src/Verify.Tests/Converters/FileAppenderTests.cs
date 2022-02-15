[UsesVerify]
public class FileAppenderTests :
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
    }

    public FileAppenderTests()
    {
        isInThisTest.Value = true;
    }

    public void Dispose()
    {
        isInThisTest.Value = false;
    }

    [Fact]
    public Task Text()
    {
        return Verify("Foo");
    }

    [Fact]
    public Task NullText()
    {
        return Verify((string) null!);
    }

    [Fact]
    public Task EmptyString()
    {
        return Verify(string.Empty);
    }

    [Fact]
    public Task Anon()
    {
        return Verify(new {foo = "bar"});
    }

    [Fact]
    public Task Stream()
    {
        return Verify(IoHelpers.OpenRead("sample.txt"));
    }

    [Fact]
    public Task File()
    {
        return VerifyFile("sample.txt");
    }
}