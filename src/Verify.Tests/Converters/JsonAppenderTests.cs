[UsesVerify]
public class JsonAppenderTests : IDisposable
{
    static AsyncLocal<bool> isInThisTest = new();

    [ModuleInitializer]
    public static void Initialize()
    {
        #region RegisterJsonAppender

        VerifierSettings.RegisterJsonAppender(
            context =>
            {
                if (ShouldInclude(context))
                {
                    return new ToAppend("theData", "theValue");
                }

                return null;
            });

        #endregion
    }

    // ReSharper disable once UnusedParameter.Local
    static bool ShouldInclude(IReadOnlyDictionary<string, object> context) =>
        isInThisTest.Value;

    public JsonAppenderTests() =>
        isInThisTest.Value = true;

    public void Dispose() =>
        isInThisTest.Value = false;

    #region JsonAppender

    [Fact]
    public Task WithJsonAppender() =>
        Verify("TheValue");

    #endregion

    #region JsonLocalAppender

    [Fact]
    public Task WithLocalJsonAppender() =>
        Verify("TheValue")
            .AppendValue("name", "value");

    #endregion

    [Fact]
    public Task WithDuplicate() =>
        Verify("TheValue")
            .AppendValue("duplicate", "value1")
            .AppendValue("duplicate", "value2");

    [Fact]
    public Task NullText() =>
        Verify((string) null!);

    [Fact]
    public Task Anon() =>
        Verify(new
        {
            foo = "bar"
        });

    #region JsonAppenderStream

    [Fact]
    public Task Stream() =>
        Verify(IoHelpers.OpenRead("sample.txt"));

    #endregion

    [Fact]
    public Task StringInfoAndStreamTarget() =>
        Verify(
            "info",
            new[]
            {
                new Target(
                    "bin",
                    new MemoryStream([1]))
            });

    [Fact]
    public Task File() =>
        VerifyFile("sample.txt");

    [Fact]
    public Task OnlyJsonAppender() =>
        Verify();

    [Fact]
    public Task NoJsonAppender()
    {
        isInThisTest.Value = false;
        return Verify();
    }
}