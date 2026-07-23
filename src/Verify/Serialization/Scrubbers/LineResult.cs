namespace VerifyTests;

/// <summary>
/// The result of processing a line via <see cref="LineReplace" />.
/// </summary>
public readonly struct LineResult
{
    internal const byte KeepKind = 0;
    internal const byte RemoveKind = 1;
    internal const byte ReplaceKind = 2;

    internal byte Kind { get; }
    internal string? Text { get; }

    LineResult(byte kind, string? text)
    {
        Kind = kind;
        Text = text;
    }

    /// <summary>
    /// Keep the line unchanged.
    /// </summary>
    public static LineResult Keep { get; } = new(KeepKind, null);

    /// <summary>
    /// Remove the line.
    /// </summary>
    public static LineResult Remove { get; } = new(RemoveKind, null);

    /// <summary>
    /// Replace the line with <paramref name="line" />.
    /// </summary>
    public static LineResult Replace(string line)
    {
        Ensure.NotNull(line);
        return new(ReplaceKind, Scrubber.NormalizeNewlines(line));
    }
}
