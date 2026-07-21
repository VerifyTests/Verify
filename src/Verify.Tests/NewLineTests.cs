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
        // The suggested .gitattributes line uses the extension of the failing file,
        // which is not always txt
        var jsonPath = CurrentFile.Relative("NewLineTests.StringWithDifferingNewline.verified.json");
        var directory = Path.GetDirectoryName(fullPath)!;
        // Globbed because received carries the namer uniqueness suffix, while the verified
        // files above are the unsuffixed fallback.
        const string receivedPattern = "NewLineTests.StringWithDifferingNewline*.received.txt";
        const string receivedJsonPattern = "NewLineTests.StringWithDifferingNewline*.received.json";
        File.Delete(fullPath);
        File.Delete(jsonPath);
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();
        // Every verify below is expected to fail, so without this the diff tool is launched
        settings.DisableDiff();

        try
        {
            // A verified file containing \r is rejected rather than silently normalized
            await File.WriteAllTextAsync(fullPath, "a\r\nb");
            var crlf = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
            Assert.Contains("carriage return", crlf.ToString());

            await File.WriteAllTextAsync(fullPath, "a\rb");
            var cr = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
            Assert.Contains("carriage return", cr.ToString());

            // The rejection writes received, so the run is not silent, and is not wrapped in a
            // generic "Failed to compare files" that hides the cause.
            foreach (var stale in Directory.EnumerateFiles(directory, receivedPattern))
            {
                File.Delete(stale);
            }

            await File.WriteAllTextAsync(fullPath, "a\r\nb");
            var rejection = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
            Assert.DoesNotContain("Failed to compare files", rejection.Message);
            Assert.Contains("*.verified.txt text eol=lf", rejection.Message);
            var received = Directory.EnumerateFiles(directory, receivedPattern).Single();
            Assert.Equal("a\nb", await File.ReadAllTextAsync(received));
            File.Delete(received);

            await File.WriteAllTextAsync(jsonPath, "{\r\n}");
            var json = await Assert.ThrowsAnyAsync<Exception>(
                () => Verify("{\n}", extension: "json", settings: settings));
            Assert.Contains("*.verified.json text eol=lf", json.Message);
            // Inline, not only in the finally, since the verifies below would otherwise treat
            // the json as a dangling verified file for this test and fail on it
            File.Delete(jsonPath);
            foreach (var stale in Directory.EnumerateFiles(directory, receivedJsonPattern))
            {
                File.Delete(stale);
            }

            // A verified file using \n still matches received content normalized to \n
            await File.WriteAllTextAsync(fullPath, "a\nb");
            await Verify("a\r\nb", settings);
            await Verify("a\rb", settings);
            await Verify("a\nb", settings);
        }
        finally
        {
            // In a finally since these deliberately contain \r. A leftover verified file is
            // normalized to \n by .gitattributes if committed, silently voiding this test.
            File.Delete(fullPath);
            File.Delete(jsonPath);
            foreach (var stale in Directory.EnumerateFiles(directory, receivedPattern)
                         .Concat(Directory.EnumerateFiles(directory, receivedJsonPattern)))
            {
                File.Delete(stale);
            }
        }
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

#if NET10_0
    [Fact]
    public async Task TrailingNewlinesRaw()
    {
        var file = CurrentFile.Relative("NewLineTests.TrailingNewlinesRaw.verified.txt");
        var directory = Path.GetDirectoryName(file)!;
        // Globbed because received carries the namer uniqueness suffix, while the verified
        // file above is the unsuffixed fallback.
        const string receivedPattern = "NewLineTests.TrailingNewlinesRaw*.received.txt";
        File.Delete(file);
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();
        // Several verifies below are expected to fail, so without this the diff tool is launched
        settings.DisableDiff();

        try
        {
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
        }
        finally
        {
            // Deleting verified orphans received, so no subsequent run reconciles it, and
            // DiffEngineTray shows it as a pending change forever.
            File.Delete(file);
            foreach (var stale in Directory.EnumerateFiles(directory, receivedPattern))
            {
                File.Delete(stale);
            }
        }
    }
#endif
}
