using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class JsonAppenderTests
{
    static JsonAppenderTests()
    {
        VerifierSettings.RegisterJsonAppender(
            settings =>
            {
                if (!settings.SourceFile.Contains("JsonAppenderTests"))
                {
                    return null;
                }

                return new ToAppend("theData", "theValue");
            });
    }

    [Fact]
    public Task Text()
    {
        return Verifier.Verify("Foo");
    }

    [Fact]
    public Task Anon()
    {
        return Verifier.Verify(new {foo = "bar"});
    }

    [Fact]
    public Task Stream()
    {
        return Verifier.Verify(FileHelpers.OpenRead("sample.txt"));
    }

    [Fact]
    public Task File()
    {
        return Verifier.VerifyFile("sample.txt");
    }
}