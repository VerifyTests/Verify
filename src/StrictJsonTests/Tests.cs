using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    static Tests()
    {
        #region UseStrictJson
        VerifierSettings.UseStrictJson();
        #endregion
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
    public async Task Object()
    {
        #region UseStrictJsonVerify
        var target = new Target
        {
            Value = "Foo"
        };
        await Verifier.Verify(target);
        #endregion
    }
}

public class Target
{
    public string? Value { get; set; }
}