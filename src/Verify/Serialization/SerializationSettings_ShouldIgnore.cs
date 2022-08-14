// ReSharper disable UseObjectOrCollectionInitializer

partial class SerializationSettings
{
    internal bool ShouldIgnore(MemberInfo member)
    {
        if (!includeObsoletes)
        {
            if (member.GetCustomAttribute<ObsoleteAttribute>(true) is not null)
            {
                return true;
            }
        }

        return ShouldIgnore(member.DeclaringType!, member.MemberType(), member.Name);
    }

    internal bool ShouldIgnore(Type declaringType, Type memberType, string name)
    {
        if (ignoredTypes.Any(memberType.InheritsFrom))
        {
            return true;
        }

        var typeFromNullable = Nullable.GetUnderlyingType(memberType);

        if (typeFromNullable != null)
        {
            if (ignoredTypes.Any(_ => _.IsAssignableFrom(typeFromNullable)))
            {
                return true;
            }
        }

        if (ignoredByNameMembers.Contains(name))
        {
            return true;
        }

        foreach (var pair in ignoredMembers)
        {
            if (pair.Value.Contains(name))
            {
                if (pair.Key.IsAssignableFrom(declaringType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal bool ShouldSerialize(object value)
    {
        var memberType = value.GetType();
        if (ignoredInstances.TryGetValue(memberType, out var funcs))
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
        if (ignoredInstances.TryGetValue(memberType, out var funcs))
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

    bool IsIgnoredCollection(Type memberType) =>
        ignoreEmptyCollections &&
        memberType.IsCollectionOrDictionary();
}