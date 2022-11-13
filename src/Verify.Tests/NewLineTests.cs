// Non-nullable field is uninitialized.

#pragma warning disable CS8618

[UsesVerify]
public class NewLineTests
{
    [Fact]
    public Task WithNewline() =>
        Verify(new
        {
            Property = "F\roo"
        });

    [Fact]
    public async Task WithRootNewlineAddedByScrubber()
    {
        var result = await Verify("value")
            .AddScrubber(_ => _.Append("\rline2\r\nline3\nline4"));
        Assert.False(File.ReadAllText(result.Files.Single()).Contains("\r"));
    }

    [Fact]
    public async Task WithNestedNewlineAddedByScrubber()
    {
        var result = await Verify(new
            {
                Property = "value"
            })
            .AddScrubber(_ => _.Append("\rline2\r\nline3\nline4"));
        Assert.False(File.ReadAllText(result.Files.Single()).Contains("\r"));
    }

#if NET6_0_OR_GREATER
    [Fact]
    public async Task StringWithDifferingNewline()
    {
        var fullPath = CurrentFile.Relative("NewLineTests.StringWithDifferingNewline.verified.txt");
        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\r\nb");
        await Verify("a\r\nb");
        PrefixUnique.Clear();
        await Verify("a\rb");
        PrefixUnique.Clear();
        await Verify("a\nb");
        PrefixUnique.Clear();

        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\nb");
        await Verify("a\r\nb");
        PrefixUnique.Clear();
        await Verify("a\rb");
        PrefixUnique.Clear();
        await Verify("a\nb");
        PrefixUnique.Clear();

        File.Delete(fullPath);
        File.WriteAllText(fullPath, "a\rb");
        await Verify("a\r\nb");
        PrefixUnique.Clear();
        await Verify("a\rb");
        PrefixUnique.Clear();
        await Verify("a\nb");
    }
#endif

    [Fact]
    public Task Newlines() =>
        Verify("a\r\nb\nc\rd\r\n");

#if NET6_0_OR_GREATER
    [Fact]
    public async Task TrailingNewlinesRaw()
    {
        var file = CurrentFile.Relative("NewLineTests.TrailingNewlinesRaw.verified.txt");
        File.Delete(file);
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();

        await File.WriteAllTextAsync(file, "a\r\n");
        await Verify("a\r\n", settings);
        await Verify("a\n", settings);
        await Verify("a", settings);

        await File.WriteAllTextAsync(file, "a\r\n\r\n");
        await Verify("a\r\n\r\n", settings);
        await Verify("a\n\n", settings);
        await Verify("a\n", settings);

        await File.WriteAllTextAsync(file, "a\n");
        await Verify("a\n", settings);
        await Verify("a", settings);

        await File.WriteAllTextAsync(file, "a\n\n");
        await Verify("a\n\n", settings);
        await Verify("a\n", settings);
        File.Delete(file);
    }

    //TODO: add test for trailing newlines
    // [Fact]
    // public async Task TrailingNewlinesObject()
    // {
    //     var file = CurrentFile.Relative("NewLineTests.TrailingNewlinesObject.verified.txt");
    //     var settings = new VerifySettings();
    //     settings.DisableRequireUniquePrefix();
    //     var target = new
    //     {
    //         s = "a"
    //     };
    //     File.WriteAllText(file, "{\n  s: a\n}");
    //     await Verify(target, settings);
    //
    //     File.WriteAllText(file, "{\n  s: a\r}");
    //     await Verify(target, settings);
    //
    //     File.WriteAllText(file, "{\n  s: a\n}\n");
    //     await Verify(target, settings);
    //
    //     File.WriteAllText(file, "{\n  s: a\n}\r\n");
    //     await Verify(target, settings);
    // }

#endif
}