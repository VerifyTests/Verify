public class CopyConstructorTests
{
    [Fact]
    public void PreservesThrowException()
    {
        var settings = new VerifySettings();
        settings.AutoVerify(throwException: true);

        var copy = new VerifySettings(settings);

        Assert.True(copy.throwException);
    }

    [Fact]
    public void ClonesAppendedFiles()
    {
        var settings = new VerifySettings();
        settings.AppendContentAsFile("base");

        var copy = new VerifySettings(settings);
        copy.AppendContentAsFile("extra");

        // The base settings must not see the file appended to the copy.
        Assert.Single(settings.appendedFiles!);
        Assert.Equal(2, copy.appendedFiles!.Count);
    }

    [Fact]
    public void ClonesExtensionStreamComparers()
    {
        var settings = new VerifySettings();
        StreamCompare compare = (_, _, _) => Task.FromResult(CompareResult.Equal);
        // Extensions unique to this test so the global comparer registry can't
        // satisfy the lookup below.
        settings.UseStreamComparer(compare, "copyctorext1");

        var copy = new VerifySettings(settings);
        copy.UseStreamComparer(compare, "copyctorext2");

        // The comparer added to the copy must not leak into the base settings.
        Assert.False(settings.TryFindStreamComparer("copyctorext2", out _));
        Assert.True(copy.TryFindStreamComparer("copyctorext2", out _));
    }

    [Fact]
    public void ClonesExtensionMappedScrubbers()
    {
        var settings = new VerifySettings();
        settings.AddScrubber("txt", _ => { });

        var copy = new VerifySettings(settings);
        copy.AddScrubber("txt", _ => { });

        // The inner list is shared unless deep-copied.
        Assert.Single(settings.ExtensionMappedInstanceScrubbers!["txt"]);
        Assert.Equal(2, copy.ExtensionMappedInstanceScrubbers!["txt"].Count);
    }
}
