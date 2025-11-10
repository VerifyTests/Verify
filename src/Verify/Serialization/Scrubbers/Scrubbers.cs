namespace VerifyTests;

public static class Scrubbers
{
    static (FrozenDictionary<string, string> exact, FrozenDictionary<string, string> replace) machineNameReplacements;
    static (FrozenDictionary<string, string> exact, FrozenDictionary<string, string> replace) userNameReplacements;

    static Scrubbers() =>
        ResetReplacements(Environment.MachineName, Environment.UserName);

    internal static void ResetReplacements(string machineName, string userName)
    {
        machineNameReplacements = CreateWrappedReplacements(machineName, "TheMachineName");
        userNameReplacements = CreateWrappedReplacements(userName, "TheUserName");
    }

    static char[] validWrappingChars =
    [
        ' ',
        '\t',
        '\n',
        '\r'
    ];

    static (FrozenDictionary<string, string> exact, FrozenDictionary<string, string> replace) CreateWrappedReplacements(string toReplace, string toReplaceWith)
    {
        var replace = new Dictionary<string, string>(validWrappingChars.Length * 2);
        foreach (var wrappingChar in validWrappingChars)
        {
            replace[wrappingChar + toReplace] = wrappingChar + toReplaceWith;
            replace[toReplace + wrappingChar] = toReplaceWith + wrappingChar;
        }

        var exact = new Dictionary<string, string>(2 + validWrappingChars.Length * validWrappingChars.Length)
        {
            {
                toReplace, toReplaceWith
            }
        };
        foreach (var beforeChar in validWrappingChars)
        foreach (var afterChar in validWrappingChars)
        {
            exact[beforeChar + toReplace + afterChar] = beforeChar + toReplaceWith + afterChar;
        }

        return (exact.ToFrozenDictionary(), replace.ToFrozenDictionary());
    }

    public static void ScrubMachineName(StringBuilder builder) =>
        PerformReplacements(builder, machineNameReplacements);

    public static void ScrubUserName(StringBuilder builder) =>
        PerformReplacements(builder, userNameReplacements);

    static void PerformReplacements(StringBuilder builder, (IReadOnlyDictionary<string, string> exact, IReadOnlyDictionary<string, string> replace) replacements)
    {
        var exactMatchingLength = replacements.exact
            .Where(_ => _.Key.Length == builder.Length)
            .ToList();
        if (exactMatchingLength.Count > 0)
        {
            var value = builder.ToString();
            foreach (var exact in exactMatchingLength)
            {
                if (value != exact.Key)
                {
                    continue;
                }

                builder.Clear();
                builder.Append(exact.Value);
                return;
            }
        }

        builder.ReplaceTokens(replacements.replace);
    }
}