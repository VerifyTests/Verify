public class GuidScrubberTests
{
    #region NamedGuidGlobal

    [ModuleInitializer]
    public static void Init() => VerifierSettings.AddNamedGuid(new("c8eeaf99-d5c4-4341-8543-4597c3fd40c9"), "guidName");

    #endregion

    static Dictionary<string, object> emptyContext = [];

    static string ReplaceGuids(string value, Counter counter)
    {
        var set = EngineScrubberSet.ForScrubbers([GuidMatcher.Instance, GuidMatcher.NInstance]);
        return ScrubEngine.Run(value, set, counter, emptyContext, applyDirectoryReplacements: false);
    }

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaa", "no match short")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c", "simple")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c 173535ae-995b-4cc6-a74e-8cd4be57039d", "simple-multiple")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c 173535ae-995b-4cc6-a74e-8cd4be57039c", "simple-duplicate")]
    [InlineData("{173535ae-995b-4cc6-a74e-8cd4be57039c}", "curly")]
    [InlineData("{173535ae-995b-4cc6-a74e-8cd4be57039c} {173535ae-995b-4cc6-a74e-8cd4be57039d}", "curly-multiple")]
    [InlineData("{173535ae-995b-4cc6-a74e-8cd4be57039c} {173535ae-995b-4cc6-a74e-8cd4be57039c}", "curly-duplicate")]
    [InlineData("{173535ae-995b-4cc6-a74e-8cd4be57039c", "start-curly")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c}", "end-curly")]
    [InlineData("-173535ae-995b-4cc6-a74e-8cd4be57039c-", "dash")]
    [InlineData("-173535ae-995b-4cc6-a74e-8cd4be57039c- -173535ae-995b-4cc6-a74e-8cd4be57039d-", "dash-multiple")]
    [InlineData("-173535ae-995b-4cc6-a74e-8cd4be57039c- -173535ae-995b-4cc6-a74e-8cd4be57039c-", "dash-duplicate")]
    [InlineData("-173535ae-995b-4cc6-a74e-8cd4be57039c", "start-dash")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c-", "end-dash")]
    [InlineData("/173535ae-995b-4cc6-a74e-8cd4be57039c/", "fslash")]
    [InlineData("/173535ae-995b-4cc6-a74e-8cd4be57039c", "start-fslash")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c/", "end-fslash")]
    [InlineData("\\173535ae-995b-4cc6-a74e-8cd4be57039c\\}", "bslash")]
    [InlineData("\\173535ae-995b-4cc6-a74e-8cd4be57039c", "start-bslash")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c\\", "end-bslash")]
    [InlineData("[173535ae-995b-4cc6-a74e-8cd4be57039c]", "square")]
    [InlineData("[173535ae-995b-4cc6-a74e-8cd4be57039c] [173535ae-995b-4cc6-a74e-8cd4be57039d]", "square-multiple")]
    [InlineData("[173535ae-995b-4cc6-a74e-8cd4be57039c] [173535ae-995b-4cc6-a74e-8cd4be57039c]", "square-duplicate")]
    [InlineData("[173535ae-995b-4cc6-a74e-8cd4be57039c", "start-square")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c]", "end-square")]
    [InlineData("a173535ae-995b-4cc6-a74e-8cd4be57039ca", "letters")]
    [InlineData("a173535ae-995b-4cc6-a74e-8cd4be57039c", "start-letters")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039ca", "end-letters")]
    [InlineData("1173535ae-995b-4cc6-a74e-8cd4be57039c1", "numbers")]
    [InlineData("1173535ae-995b-4cc6-a74e-8cd4be57039c", "start-numbers")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c1", "end-numbers")]
    [InlineData("173535ae995b4cc6a74e8cd4be57039c", "n")]
    [InlineData("173535ae995b4cc6a74e8cd4be57039c 173535ae995b4cc6a74e8cd4be57039d", "n-multiple")]
    [InlineData("173535ae995b4cc6a74e8cd4be57039c 173535ae995b4cc6a74e8cd4be57039c", "n-duplicate")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c 173535ae995b4cc6a74e8cd4be57039c", "n-d-same")]
    [InlineData("173535AE995B4CC6A74E8CD4BE57039C", "n-upper")]
    [InlineData("[173535ae995b4cc6a74e8cd4be57039c]", "n-square")]
    [InlineData("-173535ae995b4cc6a74e8cd4be57039c-", "n-dash")]
    [InlineData("a173535ae995b4cc6a74e8cd4be57039ca", "n-letters")]
    [InlineData("a173535ae995b4cc6a74e8cd4be57039c", "n-start-letters")]
    [InlineData("173535ae995b4cc6a74e8cd4be57039ca", "n-end-letters")]
    [InlineData("00000000000000000000000000000000", "n-zeros")]
    [InlineData("da39a3ee5e6b4b0d3255bfef95601890afd80709", "n-sha1")]
    [InlineData("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", "n-sha256")]
    public async Task Run(string guid, string name)
    {
        using var counter = Counter.Start();
        var result = ReplaceGuids(guid, counter);
        await Verify(result)
            .DontScrubGuids()
            .UseTextForParameters(name);
    }

    [Theory]
    [InlineData("c8eeaf99-d5c4-4341-8543-4597c3fd40c9", "named")]
    [InlineData("c8eeaf99-d5c4-4341-8543-4597c3fd40d9", "instanceNamed")]
    public Task NamedGuids(string guid, string name) =>
        Verify(
                new
                {
                    value = new Guid(guid)
                })
            .AddNamedGuid(new("c8eeaf99-d5c4-4341-8543-4597c3fd40d9"), "instanceNamed")
            .UseTextForParameters(name);

    #region NamedGuidInstance

    [Fact]
    public Task NamedGuidInstance()
    {
        var settings = new VerifySettings();
        var guid = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9");
        settings.AddNamedGuid(guid, "instanceNamed");
        return Verify(
            new
            {
                value = guid
            },
            settings);
    }

    #endregion

    //top level should not scrub
    [Fact]
    public Task NamedGuidTopLevelInstance()
    {
        var settings = new VerifySettings();
        var guid = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9");
        settings.AddNamedGuid(guid, "instanceNamed");
        return Verify(guid, settings);
    }

    [Fact]
    public void Multiple()
    {
        using var counter = Counter.Start();
        var result = ReplaceGuids("[2e6bddf7-fcf7-4b09-bb6f-a7948e1eecf3][c2eeaf99-d5c4-4341-8543-4597c3fd40d9]", counter);
        Assert.Equal("[Guid_1][Guid_2]", result);
    }

    [Fact]
    public Task InlineNamedGuidNFormat() =>
        Verify("value: c8eeaf99d5c4434185434597c3fd40c9")
            .ScrubInlineGuids();

    #region NamedGuidFluent

    [Fact]
    public Task NamedGuidFluent()
    {
        var guid = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9");
        return Verify(
                new
                {
                    value = guid
                })
            .AddNamedGuid(guid, "instanceNamed");
    }

    #endregion

    #region InferredNamedGuidFluent

    [Fact]
    public Task InferredNamedGuidFluent()
    {
        var namedGuid = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9");
        return Verify(
                new
                {
                    value = namedGuid
                })
            .AddNamedGuid(namedGuid);
    }

    #endregion

    [Fact]
    public Task NamedGuidTopLevelFluent()
    {
        var guid = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9");
        return Verify(guid)
            .AddNamedGuid(guid, "instanceNamed");
    }

    [Fact]
    public Task NamedGuidTopLevelGlobal() =>
        Verify(new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40c9"));
}
