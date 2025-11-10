static class UserMachineScrubber
{
    static string machineName;
    static string userName;

    static UserMachineScrubber() =>
        ResetReplacements(Environment.MachineName, Environment.UserName);

    [MemberNotNull(nameof(machineName), nameof(userName))]
    internal static void ResetReplacements(string machineName, string userName)
    {
        UserMachineScrubber.machineName = machineName;
        UserMachineScrubber.userName = userName;
    }

    static bool IsValidWrapper(char ch) =>
        ch is
            ' ' or
            '\t' or
            '\n' or
            '\r';

    public static void Machine(StringBuilder builder) =>
        PerformReplacements(builder, machineName, "TheMachineName");

    public static void User(StringBuilder builder) =>
        PerformReplacements(builder, userName, "TheUserName");

    static void PerformReplacements(StringBuilder builder, string find, string replace)
    {
        if (builder.Length < find.Length)
        {
            return;
        }

        var matches = FindMatches(builder, find);

        // Sort by position descending
        var orderByDescending = matches.OrderByDescending(_ => _);

        // Apply matches
        foreach (var match in orderByDescending)
        {
            builder.Overwrite(replace, match, find.Length);
        }
    }

    static IEnumerable<int> FindMatches(StringBuilder builder, string find)
    {
        var absolutePosition = 0;

        foreach (var chunk in builder.GetChunks())
        {
            if (chunk.Length < find.Length)
            {
                absolutePosition += chunk.Length;
                continue;
            }

            var chunkIndex = 0;
            while (true)
            {
                var end = chunkIndex + find.Length;
                if (end > chunk.Length)
                {
                    break;
                }

                var value = chunk.Span;
                chunkIndex = value.IndexOf(find);
                if (chunkIndex == -1)
                {
                    yield break;
                }

                if ((chunkIndex != 0 && !IsValidWrapper(value[chunkIndex - 1])) ||
                    (end != value.Length && !IsValidWrapper(value[end])))
                {
                    chunkIndex++;
                    continue;
                }

                var startReplaceIndex = absolutePosition + chunkIndex;
                yield return startReplaceIndex;
                chunkIndex += find.Length;
            }

            absolutePosition += chunk.Length;
        }
    }
}