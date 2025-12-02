static class StringBuilderChunkMatcher
{
    internal readonly struct Match(int index, int length, string value)
    {
        public readonly int Index = index;
        public readonly int Length = length;
        public readonly string Value = value;
    }

    public static void ApplyMatches(StringBuilder builder, List<Match> matches)
    {
        var orderByDescending = matches.OrderByDescending(_ => _.Index);
        foreach (var match in orderByDescending)
        {
            builder.Overwrite(match.Value, match.Index, match.Length);
        }
    }
}
