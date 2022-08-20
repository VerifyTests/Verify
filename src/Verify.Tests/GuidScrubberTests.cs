[UsesVerify]
public class GuidScrubberTests
{
    [Theory]
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
        Counter.Start();
        try
        {
            var builder = new StringBuilder(guid);
            GuidScrubber.ReplaceGuids(builder);
            await Verify(builder)
                .UseTextForParameters(name);
        }
        finally
        {
            Counter.Stop();
        }
    }
}