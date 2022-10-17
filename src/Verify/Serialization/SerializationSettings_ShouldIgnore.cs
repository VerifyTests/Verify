partial class SerializationSettings
{
    internal bool TryGetScrubOrIgnore(MemberInfo member, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        if (ShouldIgnoreIfObsolete(member))
        {
            scrubOrIgnore = ScrubOrIgnore.Ignore;
            return true;
        }

        return TryGetScrubOrIgnore(member.DeclaringType!, member.MemberType(), member.Name, out scrubOrIgnore);
    }

    internal bool TryGetScrubOrIgnore(Type declaringType, Type memberType, string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore) =>
        TryGetScrubOrIgnoreByType(memberType, out scrubOrIgnore) ||
        TryGetScrubOrIgnoreByName(name, out scrubOrIgnore) ||
        TryGetScrubOrIgnoreByMemberOfType(declaringType, name, out scrubOrIgnore);

    internal bool TryGetScrubOrIgnoreByInstance(object value, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        var memberType = value.GetType();
        if (GetShouldIgnoreInstance(memberType, out var funcs))
        {
            foreach (var func in funcs)
            {
                ScrubOrIgnore? orIgnore = func(value);
                if (orIgnore is not null)
                {
                    scrubOrIgnore = orIgnore;
                    return true;
                }
            }

            scrubOrIgnore = null;
            return false;
        }

        if (ignoreEmptyCollections)
        {
            if (value is IDictionary {Count: 0})
            {
                scrubOrIgnore = ScrubOrIgnore.Ignore;
                return true;
            }
            if (value is ICollection {Count: 0})
            {
                scrubOrIgnore = ScrubOrIgnore.Ignore;
                return true;
            }

            if (memberType.FullName?.StartsWith("System.Linq.EmptyPartition") == true)
            {
                scrubOrIgnore = ScrubOrIgnore.Ignore;
                return true;
            }
        }

        scrubOrIgnore = null;
        return false;
    }
}