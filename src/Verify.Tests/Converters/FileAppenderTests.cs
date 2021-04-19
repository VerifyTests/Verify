using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

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
        return Verifier.Verify("Foo");
    }

    [Fact]
    public Task NullText()
    {
        return Verifier.Verify((string) null!);
    }

    [Fact]
    public Task EmptyString()
    {
        return Verifier.Verify(string.Empty);
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