static class GuidScrubber
{
    public static void ReplaceGuids(StringBuilder builder, Counter counter)
    {
        if (!counter.ScrubGuids)
        {
            return;
        }

        //{173535ae-995b-4cc6-a74e-8cd4be57039c}
        if (builder.Length < 36)
        {
            return;
        }

        var context = new MatchContext(counter);

        CrossChunkMatcher.ProcessChunks(
            builder,
            carryoverSize: 35,
            context,
            OnCrossChunk,
            OnWithinChunk);

        CrossChunkMatcher.ApplyMatches(
            builder,
            context.Matches,
            getIndex: m => m.Index,
            getLength: _ => 36,
            getValue: m => m.Value);
    }

    static void OnCrossChunk(
        StringBuilder builder,
        Span<char> carryoverBuffer,
        int carryoverIndex,
        int remainingInCarryover,
        CharSpan currentChunkSpan,
        int absoluteStartPosition,
        MatchContext context)
    {
        var neededFromCurrent = 36 - remainingInCarryover;

        if (neededFromCurrent <= 0 ||
            currentChunkSpan.Length < neededFromCurrent)
        {
            return;
        }

        // Validate start boundary
        if (absoluteStartPosition > 0 &&
            IsInvalidStartingChar(builder[absoluteStartPosition - 1]))
        {
            return;
        }

        // Validate end boundary
        if (neededFromCurrent < currentChunkSpan.Length &&
            IsInvalidEndingChar(currentChunkSpan[neededFromCurrent]))
        {
            return;
        }

        // Combine carryover and current chunk into buffer
        Span<char> buffer = stackalloc char[36];
        carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(buffer);
        currentChunkSpan[..neededFromCurrent].CopyTo(buffer[remainingInCarryover..]);

        if (!Guid.TryParseExact(buffer, "D", out var guid))
        {
            return;
        }

        var convert = context.Counter.Convert(guid);
        context.Matches.Add(new(absoluteStartPosition, convert));
    }

    static int OnWithinChunk(
        ReadOnlyMemory<char> chunk,
        CharSpan chunkSpan,
        int chunkIndex,
        int absoluteIndex,
        MatchContext context)
    {
        var end = chunkIndex + 36;
        if (end > chunk.Length)
        {
            return 1;
        }

        // Validate boundaries
        if (chunkIndex > 0 && IsInvalidStartingChar(chunkSpan[chunkIndex - 1]))
        {
            return 1;
        }

        if (end < chunkSpan.Length && IsInvalidEndingChar(chunkSpan[end]))
        {
            return 1;
        }

        var slice = chunkSpan.Slice(chunkIndex, 36);

        if (!Guid.TryParseExact(slice, "D", out var guid))
        {
            return 1;
        }

        var convert = context.Counter.Convert(guid);
        context.Matches.Add(new(absoluteIndex, convert));
        return 36; // Skip past the matched GUID
    }

    static bool IsInvalidEndingChar(char ch) =>
        IsInvalidChar(ch) &&
        ch != '}' &&
        ch != ')';

    static bool IsInvalidChar(char ch) =>
        char.IsLetter(ch) ||
        char.IsNumber(ch);

    static bool IsInvalidStartingChar(char ch) =>
        IsInvalidChar(ch) &&
        ch != '{' &&
        ch != '(';

    sealed class MatchContext(Counter counter)
    {
        public Counter Counter { get; } = counter;
        public List<Match> Matches { get; } = [];
    }

    internal readonly struct Match(int index, string value)
    {
        public readonly int Index = index;
        public readonly string Value = value;
    }
}
