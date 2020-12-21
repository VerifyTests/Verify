using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    static Tests()
    {
        VerifierSettings.UseStrictJson();
    }

    [Fact]
    public Task String()
    {
        return Verifier.Verify("Foo");
    }

    [Fact]
    public Task Dynamic()
    {
        return Verifier.Verify(new {value = "Foo"});
    }

    [Fact]
    public Task Object()
    {
        return Verifier.Verify(new Target {Value = "Foo"});
    }
}

public class Target
{
    public string? Value { get; set; }
}