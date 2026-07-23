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

    [Fact]
    public async Task StringWithDifferingNewline()
    {
        // This writes verified files into the source directory and mutates them repeatedly,
        // so every target framework has to own a distinct set of paths. UniqueForRuntimeAndVersion
        // puts the runtime in the name, which is what allows this to run on all of them at once.
        var prefix = $"NewLineTests.StringWithDifferingNewline.{Namer.RuntimeAndVersion}";
        var verifiedPath = CurrentFile.Relative($"{prefix}.verified.txt");
        var receivedPath = CurrentFile.Relative($"{prefix}.received.txt");
        // The suggested .gitattributes line uses the extension of the failing file,
        // which is not always txt
        var verifiedJsonPath = CurrentFile.Relative($"{prefix}.verified.json");
        var receivedJsonPath = CurrentFile.Relative($"{prefix}.received.json");
        File.Delete(verifiedPath);
        File.Delete(verifiedJsonPath);
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
        settings.DisableRequireUniquePrefix();
        // Every verify below is expected to fail, so without this the diff tool is launched
        settings.DisableDiff();

        try
        {
            // A verified file containing \r is rejected rather than silently normalized
            await File.WriteAllTextAsync(verifiedPath, "a\r\nb");
            var crlf = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
            Assert.Contains("carriage return", crlf.ToString());

            await File.WriteAllTextAsync(verifiedPath, "a\rb");
            var cr = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
            Assert.Contains("carriage return", cr.ToString());

            // The rejection writes received, so the run is not silent, and is not wrapped in a
            // generic "Failed to compare files" that hides the cause.
            File.Delete(receivedPath);

            await File.WriteAllTextAsync(verifiedPath, "a\r\nb");
            var rejection = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", settings));
            Assert.DoesNotContain("Failed to compare files", rejection.Message);
            Assert.Contains("*.verified.txt text eol=lf", rejection.Message);
            Assert.Equal("a\nb", await File.ReadAllTextAsync(receivedPath));
            File.Delete(receivedPath);

            await File.WriteAllTextAsync(verifiedJsonPath, "{\r\n}");
            var json = await Assert.ThrowsAnyAsync<Exception>(
                () => Verify("{\n}", extension: "json", settings: settings));
            Assert.Contains("*.verified.json text eol=lf", json.Message);
            // Inline, not only in the finally, since the verifies below would otherwise treat
            // the json as a dangling verified file for this test and fail on it
            File.Delete(verifiedJsonPath);
            File.Delete(receivedJsonPath);

            // A verified file using \n still matches received content normalized to \n
            await File.WriteAllTextAsync(verifiedPath, "a\nb");
            await Verify("a\r\nb", settings);
            await Verify("a\rb", settings);
            await Verify("a\nb", settings);
        }
        finally
        {
            // In a finally since these deliberately contain \r. A leftover verified file is
            // normalized to \n by .gitattributes if committed, silently voiding this test.
            File.Delete(verifiedPath);
            File.Delete(receivedPath);
            File.Delete(verifiedJsonPath);
            File.Delete(receivedJsonPath);
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

    [Fact]
    public async Task TrailingNewlinesRaw()
    {
        // Per target framework paths, for the same reason as StringWithDifferingNewline above
        var prefix = $"NewLineTests.TrailingNewlinesRaw.{Namer.RuntimeAndVersion}";
        var file = CurrentFile.Relative($"{prefix}.verified.txt");
        var receivedPath = CurrentFile.Relative($"{prefix}.received.txt");
        File.Delete(file);
        var settings = new VerifySettings();
        settings.UniqueForRuntimeAndVersion();
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
            File.Delete(receivedPath);
        }
    }
}
