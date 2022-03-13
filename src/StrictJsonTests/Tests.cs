using Newtonsoft.Json;

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
    public Task String() =>
        Verify("Foo");

    [Fact]
    public async Task JsonAsString()
    {
        var json = JsonConvert.SerializeObject(new
        {
            Prop1 = "string value",
            Prop2 = 123
        }, Formatting.Indented);

        await Verify(json);

        var projectDir = Directory.GetParent(AppContext.BaseDirectory)
            ?.Parent?.Parent?.Parent?.FullName;

        if (projectDir == null)
            throw new InvalidOperationException("Project directory not found");

        var file = Directory.EnumerateFiles(projectDir)
            .Select(Path.GetFileName)
            .Single(x => x?.StartsWith("Tests.String.") == true);

        Assert.True(VerifierSettings.StrictJson);

        var extension = Path.GetExtension(file);
        Assert.Equal(".json", extension);
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
    public Task Dynamic() =>
        Verify(new {value = "Foo"});

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