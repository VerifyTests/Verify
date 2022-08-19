partial class SerializationSettings
{
    internal bool ShouldIgnore(MemberInfo member, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore)
    {
        if (ShouldIgnoreIfObsolete(member))
        {
            scrubOrIgnore = ScrubOrIgnore.Ignore;
            return true;
        }

        return ShouldIgnore(member.DeclaringType!, member.MemberType(), member.Name, out scrubOrIgnore);
    }

    internal bool ShouldIgnore(Type declaringType, Type memberType, string name, [NotNullWhen(true)] out ScrubOrIgnore? scrubOrIgnore) =>
        ShouldIgnoreType(memberType, out scrubOrIgnore) ||
        ShouldIgnoreByName(name, out scrubOrIgnore) ||
        ShouldIgnoreForMemberOfType(declaringType, name, out scrubOrIgnore);

    internal bool ShouldSerialize(object value, [NotNullWhen(false)] out ScrubOrIgnore? scrubOrIgnore)
    {
        var memberType = value.GetType();
        if (GetShouldIgnoreInstance(memberType, out var funcs))
        {
            foreach (var func in funcs)
            {
                ScrubOrIgnore? orIgnore = func(value);
                if (orIgnore != null)
                {
                    scrubOrIgnore = orIgnore;
                    return false;
                }
            }

            scrubOrIgnore = null;
            return true;
        }

        if (IsIgnoredCollection(memberType))
        {
            // since inside IsCollection, it is safe to use IEnumerable
            var collection = (IEnumerable) value;
            if (!collection.HasMembers())
            {
                scrubOrIgnore = ScrubOrIgnore.Ignore;
                return false;
            }
        }

        scrubOrIgnore = null;
        return true;
    }

    internal bool TryGetShouldSerialize(Type memberType, Func<object, object?> getValue, out Predicate<object>? shouldSerialize)
    {
        if (GetShouldIgnoreInstance(memberType, out var funcs))
        {
            shouldSerialize = declaringInstance =>
            {
                var instance = getValue(declaringInstance);

                if (instance is null)
                {
                    return false;
                }

                return funcs.All(func => !func(instance));
            };

            return true;
        }

        if (IsIgnoredCollection(memberType))
        {
            shouldSerialize = declaringInstance =>
            {
                var instance = getValue(declaringInstance);

                if (instance is null)
                {
                    return false;
                }

                // since inside IsCollection, it is safe to use IEnumerable
                var collection = (IEnumerable) instance;

                return collection.HasMembers();
            };

            return true;
        }

        shouldSerialize = null;
        return false;
    }
 }