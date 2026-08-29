partial class SerializationSettings
{
    internal bool TryGetScrubOrIgnore(MemberInfo member, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        if (ShouldIgnoreIfObsolete(member))
        {
            scrubOrIgnore = ScrubOrIgnore.Ignore;
            return true;
        }

        return TryGetScrubOrIgnore(member.DeclaringType!, member.MemberType(), member.Name, member, out scrubOrIgnore);
    }

    internal bool TryGetScrubOrIgnore(Type declaringType, Type memberType, string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore) =>
        TryGetScrubOrIgnore(declaringType, memberType, name, null, out scrubOrIgnore);

    internal bool TryGetScrubOrIgnore(Type declaringType, Type memberType, string name, MemberInfo? memberInfo, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore) =>
        TryGetScrubOrIgnoreByType(memberType, out scrubOrIgnore) ||
        TryGetScrubOrIgnoreByName(name, out scrubOrIgnore) ||
        TryGetScrubOrIgnoreByMemberOfType(declaringType, name, out scrubOrIgnore) ||
        TryGetScrubOrIgnorePredicateByName(name, memberInfo, out scrubOrIgnore);

    internal bool TryGetScrubOrIgnoreByInstance(object value, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        var memberType = value.GetType();
        // no predicate matching is not a decision to keep the value, so
        // the empty collection check still applies
        if (GetShouldIgnoreInstance(memberType, out var funcs))
        {
            foreach (var func in funcs)
            {
                var orIgnore = func(value);
                if (orIgnore is not null)
                {
                    scrubOrIgnore = orIgnore;
                    return true;
                }
            }
        }

        if (ignoreEmptyCollections &&
            value.IsEmptyCollectionOrDictionary())
        {
            scrubOrIgnore = ScrubOrIgnore.Ignore;
            return true;
        }

        scrubOrIgnore = null;
        return false;
    }
}