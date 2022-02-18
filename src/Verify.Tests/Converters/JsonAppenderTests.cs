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
    static bool ShouldInclude(IReadOnlyDictionary<string, object> context)
    {
        return isInThisTest.Value;
    }

    public JsonAppenderTests()
    {
        isInThisTest.Value = true;
    }

    public void Dispose()
    {
        isInThisTest.Value = false;
    }

    #region JsonAppender

    [Fact]
    public Task WithJsonAppender()
    {
        return Verify("TheValue");
    }

    #endregion

    #region JsonLocalAppender

    [Fact]
    public Task WithLocalJsonAppender()
    {
        return Verify("TheValue")
            .AppendValue("name", "value");
    }

    #endregion

    [Fact]
    public Task NullText()
    {
        return Verify((string) null!);
    }

    [Fact]
    public Task Anon()
    {
        return Verify(new {foo = "bar"});
    }

    #region JsonAppenderStream
    [Fact]
    public Task Stream()
    {
        return Verify(IoHelpers.OpenRead("sample.txt"));
    }
    #endregion

    [Fact]
    public Task File()
    {
        return VerifyFile("sample.txt");
    }
}