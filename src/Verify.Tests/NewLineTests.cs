// Non-nullable field is uninitialized.

#pragma warning disable CS8618

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
        var file = result.Files.Single();
        Assert.DoesNotContain(
            "\r",
            await File.ReadAllTextAsync(file));
    }

    [Fact]
    public async Task WithNestedNewlineAddedByScrubber()
    {
        var result = await Verify(new
            {
                Property = "value"
            })
            .AddScrubber(_ => _.Append("\rline2\r\nline3\nline4"));
        var file = result.Files.Single();
        Assert.DoesNotContain("\r", await File.ReadAllTextAsync(file));
    }

    [Fact]
    public Task Newlines() =>
        Verify("a\r\nb\nc\rd\r\n");

#if NET9_0
    [Fact]
    public async Task StringWithDifferingNewline()
    {
        var fullPath = CurrentFile.Relative("NewLineTests.StringWithDifferingNewline.verified.txt");
        File.Delete(fullPath);
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();

        // A verified file containing \r is rejected rather than silently normalized
        await File.WriteAllTextAsync(fullPath, "a\r\nb");
        var crlf = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
        Assert.Contains("carriage return", crlf.ToString());

        await File.WriteAllTextAsync(fullPath, "a\rb");
        var cr = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
        Assert.Contains("carriage return", cr.ToString());

        // A verified file using \n still matches received content normalized to \n
        await File.WriteAllTextAsync(fullPath, "a\nb");
        await Verify("a\r\nb", settings);
        await Verify("a\rb", settings);
        await Verify("a\nb", settings);

        File.Delete(fullPath);
    }

    [Fact]
    public async Task TrailingNewlinesRaw()
    {
        var file = CurrentFile.Relative("NewLineTests.TrailingNewlinesRaw.verified.txt");
        File.Delete(file);
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();

        // A verified file containing \r is rejected
        await File.WriteAllTextAsync(file, "a\r\n");
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\n", settings));
        Assert.Contains("carriage return", exception.ToString());

        // Trailing newlines are now compared exactly, with no tolerance
        await File.WriteAllTextAsync(file, "a\n");
        await Verify("a\n", settings);
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", settings));

        await File.WriteAllTextAsync(file, "a\n\n");
        await Verify("a\n\n", settings);
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a\n", settings));
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