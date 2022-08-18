class OrderedStringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, object?>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public OrderedStringDictionaryWrapper(Func<string, ScrubOrIgnore?> shouldIgnore, TInner inner) :
        base(BuildInner(shouldIgnore, inner))
    {
    }

    static Dictionary<string, object?> BuildInner(Func<string, ScrubOrIgnore?> shouldIgnore, TInner inner)
    {
        var dictionary = new Dictionary<string, object?>();
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
                dictionary.Add(key, "{Scrubbed}");
                continue;
            }

            dictionary.Add(key, pair.Value);
        }

        return dictionary;
    }
}