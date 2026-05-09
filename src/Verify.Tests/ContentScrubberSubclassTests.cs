public class ContentScrubberSubclassTests
{
    sealed class WrapBraces : ContentScrubber
    {
        public override void Process(
            ReadOnlySpan<char> input,
            StringBuilder output,
            Counter counter,
            IReadOnlyDictionary<string, object> context)
        {
            output.Append('{');
            output.Append(input);
            output.Append('}');
        }
    }

    sealed class AppendSuffix(string suffix) : ContentScrubber
    {
        public override void Process(
            ReadOnlySpan<char> input,
            StringBuilder output,
            Counter counter,
            IReadOnlyDictionary<string, object> context)
        {
            output.Append(input);
            output.Append(suffix);
        }
    }

    [Fact]
    public Task SimpleContentTransform()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new WrapBraces());
        return Verify("hello", settings);
    }

    [Fact]
    public Task MultipleContentScrubbersInRegistrationOrder()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new AppendSuffix(" one"));
        settings.AddScrubber(new AppendSuffix(" two"));
        return Verify("line", settings);
    }

    [Fact]
    public Task ContentScrubberRunsBeforePatternScrubber()
    {
        // ContentScrubber wraps in braces, then a PatternScrubber rewrites '{' to '['.
        // If the order were reversed, no '{' would exist when the pattern ran.
        var settings = new VerifySettings();
        settings.AddScrubber(new WrapBraces());
        settings.AddScrubber(new PatternScrubberTests_BraceRewriter());
        return Verify("inner", settings);
    }

    [Fact]
    public Task EmptyInputContentScrubber()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(new AppendSuffix("X"));
        return Verify("", settings);
    }
}

sealed class PatternScrubberTests_BraceRewriter : PatternScrubber
{
    public override int MinLength => 1;
    public override int MaxLength => 1;
    public override bool SingleLine => false;

    public override bool TryMatch(
        ReadOnlySpan<char> source,
        int position,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchLength,
        [NotNullWhen(true)] out string? matched)
    {
        var ch = source[position];
        if (ch == '{')
        {
            matchLength = 1;
            matched = "[";
            return true;
        }

        if (ch == '}')
        {
            matchLength = 1;
            matched = "]";
            return true;
        }

        matchLength = 0;
        matched = null;
        return false;
    }
}
