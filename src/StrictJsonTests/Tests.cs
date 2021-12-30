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
        return Verify("Foo");
    }

    [Fact]
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonArrayString()
    {
        var json = "[{'key': {'msg': 'This is a proper json string'}}, {'key': {'msg': 'Foo'}}]";
        return VerifyJson(json);
    }

    [Fact]
    public Task Dynamic()
    {
        return Verify(new {value = "Foo"});
    }

    [Fact]
    public async Task Object()
    {
        #region UseStrictJsonVerify

        var target = new TheTarget
        {
            Value = "Foo"
        };
        await Verify(target);

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
        return Verify(new MemoryStream())
            .UseExtension("foo");
    }
}

public class TheTarget
{
    public string? Value { get; set; }
}