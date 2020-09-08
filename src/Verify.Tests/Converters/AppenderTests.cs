using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class AppenderTests
{
    static AppenderTests()
    {
        VerifierSettings.RegisterAppender(
            settings =>
            {
                if (settings.SourceFile.Contains("AppenderTests"))
                {
                    return new ConversionStream("txt", "data");
                }

                return null;
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