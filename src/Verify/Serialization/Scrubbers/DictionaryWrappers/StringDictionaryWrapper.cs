class StringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, object?>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public StringDictionaryWrapper(Func<string, ScrubOrIgnore?> shouldIgnore, TInner inner) :
        base(BuildInner(shouldIgnore, inner)
            .ToDictionary(_ => _.Key, _ => _.Value))
    {
    }

    static IEnumerable<(string Key, object? Value)> BuildInner(Func<string, ScrubOrIgnore?> shouldIgnore, TInner inner)
    {
        foreach (var pair in inner)
        {
            var key = pair.Key;
            var scrubOrIgnore = shouldIgnore(key);
            if (scrubOrIgnore == ScrubOrIgnore.Ignore)
            {
                continue;
            }

            if (scrubOrIgnore == ScrubOrIgnore.Scrub)
            {
                yield return (key, "{Scrubbed}");
                continue;
            }

            yield return (key, pair.Value);
        }
    }
}