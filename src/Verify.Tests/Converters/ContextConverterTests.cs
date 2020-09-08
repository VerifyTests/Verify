using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ContextConverterTests
{
    static ContextConverterTests()
    {
        VerifierSettings.RegisterContextConverter(
            settings =>
            {
                return new ConversionStream("txt", "data");
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