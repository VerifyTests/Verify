[UsesVerify]
public class GuidScrubberTests
{
    #region NamedGuidGlobal

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddNamedGuid(new("c8eeaf99-d5c4-4341-8543-4597c3fd40c9"), "guidName");

    #endregion

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaa", "no match short")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c", "simple")]
    [InlineData("{173535ae-995b-4cc6-a74e-8cd4be57039c}", "curly")]
    [InlineData("{173535ae-995b-4cc6-a74e-8cd4be57039c", "start-curly")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c}", "end-curly")]
    [InlineData("-173535ae-995b-4cc6-a74e-8cd4be57039c-", "dash")]
    [InlineData("-173535ae-995b-4cc6-a74e-8cd4be57039c", "start-dash")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c-", "end-dash")]
    [InlineData("/173535ae-995b-4cc6-a74e-8cd4be57039c/", "fslash")]
    [InlineData("/173535ae-995b-4cc6-a74e-8cd4be57039c", "start-fslash")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c/", "end-fslash")]
    [InlineData("\\173535ae-995b-4cc6-a74e-8cd4be57039c\\}", "bslash")]
    [InlineData("\\173535ae-995b-4cc6-a74e-8cd4be57039c", "start-bslash")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c\\", "end-bslash")]
    [InlineData("[173535ae-995b-4cc6-a74e-8cd4be57039c]", "square")]
    [InlineData("[173535ae-995b-4cc6-a74e-8cd4be57039c", "start-square")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c]", "end-square")]
    [InlineData("a173535ae-995b-4cc6-a74e-8cd4be57039ca", "letters")]
    [InlineData("a173535ae-995b-4cc6-a74e-8cd4be57039c", "start-letters")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039ca", "end-letters")]
    [InlineData("1173535ae-995b-4cc6-a74e-8cd4be57039c1", "numbers")]
    [InlineData("1173535ae-995b-4cc6-a74e-8cd4be57039c", "start-numbers")]
    [InlineData("173535ae-995b-4cc6-a74e-8cd4be57039c1", "end-numbers")]
    public async Task Run(string guid, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(guid);
            GuidScrubber.ReplaceGuids(builder, counter);
            await Verify(builder)
                .UseTextForParameters(name);
        }
        finally
        {
            Counter.Stop();
        }
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
        settings.AddNamedGuid(new("c8eeaf99-d5c4-4341-8543-4597c3fd40d9"), "instanceNamed");
        return Verify(
            new
            {
                value = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9")
            },
            settings);
    }

    #endregion

    #region NamedGuidFluent

    [Fact]
    public Task NamedGuidFluent() =>
        Verify(
                new
                {
                    value = new Guid("c8eeaf99-d5c4-4341-8543-4597c3fd40d9")
                })
            .AddNamedGuid(new("c8eeaf99-d5c4-4341-8543-4597c3fd40d9"), "instanceNamed");

    #endregion
}