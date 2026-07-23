// The span based path matcher shared by the engine pass and the legacy StringBuilder pass.
// Matching rules: '/' and '\' are equivalent (Find is pre-sanitized to '/'); the char before a match
// must not be a letter or digit; a trailing letter or digit invalidates the match; a trailing '/' or
// '\' within the current segment is greedily absorbed into the match. Pairs are ordered longest
// first, so at a given position the most specific path wins.
static partial class DirectoryReplacements
{
    // Attempts to match any pair at the given position within the span.
    // beforeSegment/afterSegment are the neighbor chars when the position touches the
    // segment edge (null at the document edge). Greedy trailing separator absorption
    // never crosses the segment boundary.
    internal static bool TryMatchAt(
        CharSpan span,
        int position,
        char? beforeSegment,
        char? afterSegment,
        List<Pair> pairs,
        out int matchLength,
        out string replacement)
    {
        matchLength = 0;
        replacement = string.Empty;

        var preceding = position > 0 ? span[position - 1] : beforeSegment;
        if (preceding is { } precedingChar &&
            char.IsLetterOrDigit(precedingChar))
        {
            return false;
        }

        foreach (var pair in pairs)
        {
            var find = pair.Find;
            var end = position + find.Length;
            if (end > span.Length)
            {
                continue;
            }

            if (!IsPathMatch(span.Slice(position, find.Length), find))
            {
                continue;
            }

            if (end < span.Length)
            {
                var trailing = span[end];
                if (char.IsLetterOrDigit(trailing))
                {
                    continue;
                }

                matchLength = find.Length;

                // Greedy: include a trailing separator
                if (trailing is '/' or '\\')
                {
                    matchLength++;
                }
            }
            else
            {
                if (afterSegment is { } after &&
                    char.IsLetterOrDigit(after))
                {
                    continue;
                }

                matchLength = find.Length;
            }

            replacement = pair.Replace;
            return true;
        }

        return false;
    }

    // Treat '/' and '\' as equivalent. Find never contains '\'.
    static bool IsPathMatch(CharSpan candidate, string find)
    {
        for (var index = 0; index < find.Length; index++)
        {
            var candidateChar = candidate[index];
            var findChar = find[index];

            if (candidateChar is '/' or '\\')
            {
                if (findChar != '/')
                {
                    return false;
                }

                continue;
            }

            if (candidateChar != findChar)
            {
                return false;
            }
        }

        return true;
    }
}
