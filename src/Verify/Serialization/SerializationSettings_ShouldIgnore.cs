partial class SerializationSettings
{
    internal bool ShouldIgnore(MemberInfo member)
    {
        if (ShouldIgnoreIfObsolete(member))
        {
            return true;
        }

        return ShouldIgnore(member.DeclaringType!, member.MemberType(), member.Name);
    }

    internal bool ShouldIgnore(Type declaringType, Type memberType, string name)
    {
        if (ShouldIgnoreType(memberType))
        {
            return true;
        }

        if (ShouldIgnoreByName(name))
        {
            return true;
        }

        if (ShouldIgnoreForMemberOfType(declaringType, name))
        {
            return true;
        }

        return false;
    }

    internal bool ShouldSerialize(object value)
    {
        var memberType = value.GetType();
        if (GetShouldIgnoreInstance(memberType, out var funcs))
        {
            return funcs.All(func => !func(value));
        }

        if (IsIgnoredCollection(memberType))
        {
            // since inside IsCollection, it is safe to use IEnumerable
            var collection = (IEnumerable) value;

            return collection.HasMembers();
        }

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