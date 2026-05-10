public class LineScrubberSubclassTests
{
    sealed class DropLines(params string[] dropContaining) : LineScrubber
    {
        public override bool Process(
            ReadOnlySpan<char> line,
            Counter counter,
            IReadOnlyDictionary<string, object> context,
            out string? replacement)
        {
            replacement = null;
            foreach (var token in dropContaining)
            {
                if (line.Contains(token.AsSpan(), StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }
    }

    sealed class UppercaseLines : LineScrubber
    {
        public override bool Process(
            ReadOnlySpan<char> line,
            Counter counter,
            IReadOnlyDictionary<string, object> context,
            out string? replacement)
        {
            replacement = line.ToString().ToUpperInvariant();
            return true;
        }
    }

    [Fact]
    public Task DropMatchingLines()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new DropLines("drop"));
        return Verify("keep\ndrop me\nkeep too", settings);
    }

    [Fact]
    public Task ChainedLineScrubbersInRegistrationOrder()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new DropLines("internal:"));
        settings.AddScrubber(new UppercaseLines());
        return Verify("public: hello\ninternal: secret\npublic: world", settings);
    }

    [Fact]
    public Task LineScrubberWithCrlfEndings()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new DropLines("two"));
        return Verify("one\r\ntwo\r\nthree", settings);
    }

    [Fact]
    public Task LineScrubberWithBareCr()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new DropLines("middle"));
        return Verify("first\rmiddle\rlast", settings);
    }

    [Fact]
    public Task DroppedLastLineDoesNotLeaveTrailingTerminator()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new DropLines("dropme"));
        return Verify("keep\ndropme", settings);
    }
}
