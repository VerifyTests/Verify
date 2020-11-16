using System;
using System.Threading;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class JsonAppenderTests : IDisposable
{
    static AsyncLocal<bool> isInThisTest = new();

    static JsonAppenderTests()
    {
        VerifierSettings.RegisterJsonAppender(
            _ =>
            {
                if (!isInThisTest.Value)
                {
                    return null;
                }

                return new ToAppend("theData", "theValue");
            });
    }

    public JsonAppenderTests()
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
        return Verifier.Verify("Foo");
    }

    [Fact]
    public Task NullText()
    {
        return Verifier.Verify((string) null!);
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