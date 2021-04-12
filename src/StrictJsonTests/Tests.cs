using System.IO;
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
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return Verifier.VerifyJson(json);
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

        TheTarget target = new()
        {
            Value = "Foo"
        };
        await Verifier.Verify(target);

        #endregion
    }

    [Fact]
    public Task WithInfo()
    {
        VerifierSettings.RegisterFileConverter(
            "foo",
            (_, _) =>
            {
                var info = new
                {
                    Property = "Value"
                };
                return new(info, "txt", "content");
            });
        return Verifier.Verify(new MemoryStream())
            .UseExtension("foo");
    }
}

public class TheTarget
{
    public string? Value { get; set; }
}