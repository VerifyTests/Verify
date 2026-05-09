public class PatternScrubberTests
{
    sealed class TagScrubber(string tag, string replacement, int min, int max, bool singleLine = true) : PatternScrubber
    {
        public int MatchCalls;

        public override int MinLength => min;
        public override int MaxLength => max;
        public override bool SingleLine => singleLine;

        public override bool TryMatch(
            ReadOnlySpan<char> source,
            int position,
            Counter counter,
            IReadOnlyDictionary<string, object> context,
            out int matchLength,
            [NotNullWhen(true)] out string? matched)
        {
            MatchCalls++;
            if (position + tag.Length > source.Length)
            {
                matchLength = 0;
                matched = null;
                return false;
            }

            if (!source.Slice(position, tag.Length).SequenceEqual(tag.AsSpan()))
            {
                matchLength = 0;
                matched = null;
                return false;
            }

            matchLength = tag.Length;
            matched = replacement;
            return true;
        }
    }

    [Fact]
    public Task SimpleMatch()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new TagScrubber("foo", "{FOO}", 3, 3));
        return Verify("the foo is foo", settings);
    }

    [Fact]
    public async Task SkipWhenInputShorterThanMinLength()
    {
        var scrubber = new TagScrubber("never", "{X}", 50, 50);
        var settings = new VerifySettings();
        settings.AddScrubber(scrubber);

        await Verify("short", settings);
        Assert.Equal(0, scrubber.MatchCalls);
    }

    [Fact]
    public Task LongerMatchWinsOverlap()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new TagScrubber("ab", "{SHORT}", 2, 2));
        settings.AddScrubber(new TagScrubber("abcd", "{LONG}", 4, 4));
        return Verify("abcd", settings);
    }

    [Fact]
    public async Task NonChainableAtPosition()
    {
        // First scrubber matches "a" -> "{A}". Second scrubber would match "a"
        // -> "{B}" but isn't tried at the same position because the first claimed it.
        var second = new TagScrubber("a", "{B}", 1, 1);
        var settings = new VerifySettings();
        settings.AddScrubber(new TagScrubber("a", "{A}", 1, 1));
        settings.AddScrubber(second);
        await Verify("a", settings);
        // Both scrubbers have MaxLength=1, but only one wins per position;
        // the second's TryMatch must not have been invoked at position 0.
        Assert.Equal(0, second.MatchCalls);
    }

    [Fact]
    public async Task SingleLineSkipsLinesShorterThanMin()
    {
        var scrubber = new TagScrubber("xxxxxxxx", "{LONG}", 8, 8, singleLine: true);
        var settings = new VerifySettings();
        settings.AddScrubber(scrubber);
        // Input has 4-char lines and one 8-char line; only the 8-char line is visited
        // for matching. The shorter lines are skipped wholesale by the per-line filter.
        await Verify("abcd\nxxxxxxxx\nefgh", settings);
        // The 8-char line gets at least one match attempt; without the per-line
        // skip, the 4-char lines would also rack up calls.
        Assert.True(scrubber.MatchCalls >= 1);
        Assert.True(scrubber.MatchCalls <= 8);
    }

    sealed class BoundaryAwareScrubber : PatternScrubber
    {
        public override int MinLength => 3;
        public override int MaxLength => 3;
        public override bool SingleLine => true;

        public override bool TryMatch(
            ReadOnlySpan<char> source,
            int position,
            Counter counter,
            IReadOnlyDictionary<string, object> context,
            out int matchLength,
            [NotNullWhen(true)] out string? matched)
        {
            if (position + 3 > source.Length)
            {
                matchLength = 0;
                matched = null;
                return false;
            }

            if (!source.Slice(position, 3).SequenceEqual("foo".AsSpan()))
            {
                matchLength = 0;
                matched = null;
                return false;
            }

            // Reject if surrounded by letters on either side.
            if (position > 0 && char.IsLetter(source[position - 1]))
            {
                matchLength = 0;
                matched = null;
                return false;
            }

            if (position + 3 < source.Length && char.IsLetter(source[position + 3]))
            {
                matchLength = 0;
                matched = null;
                return false;
            }

            matchLength = 3;
            matched = "{FOO}";
            return true;
        }
    }

    [Fact]
    public Task BoundaryCheckUsesSurroundingChars()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new BoundaryAwareScrubber());
        return Verify("foo barfoo foox foo!", settings);
    }
}
