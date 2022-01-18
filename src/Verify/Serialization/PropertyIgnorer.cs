class PropertyIgnorer
{
    bool ignoreEmptyCollections;
    bool includeObsoletes;
    IReadOnlyDictionary<Type, List<string>> ignoredMembers;
    IReadOnlyList<string> ignoredByNameMembers;
    IReadOnlyList<Type> ignoredTypes;
    IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances;

    public PropertyIgnorer(
        bool ignoreEmptyCollections,
        bool includeObsoletes,
        IReadOnlyDictionary<Type, List<string>> ignoredMembers,
        IReadOnlyList<string> ignoredByNameMembers,
        IReadOnlyList<Type> ignoredTypes,
        IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances)
    {
        this.ignoreEmptyCollections = ignoreEmptyCollections;
        this.includeObsoletes = includeObsoletes;
        this.ignoredMembers = ignoredMembers;
        this.ignoredByNameMembers = ignoredByNameMembers;
        this.ignoredTypes = ignoredTypes;
        this.ignoredInstances = ignoredInstances;
    }

    public bool ShouldIgnore(MemberInfo member)
    {
        if (!includeObsoletes)
        {
            if (member.GetCustomAttribute<ObsoleteAttribute>(true) is not null)
            {
                return true;
            }
        }

        var propertyType = member.MemberType();
        if (ignoredTypes.Any(x => x.IsAssignableFrom(propertyType)))
        {
            return true;
        }

        if (ignoredByNameMembers.Contains(member.Name))
        {
            return true;
        }

        foreach (var pair in ignoredMembers)
        {
            if (pair.Value.Contains(member.Name))
            {
                if (pair.Key.IsAssignableFrom(member.DeclaringType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool TryGetShouldSerialize(Type propertyType, Func<object, object?> getValue, out Predicate<object>? shouldSerialize)
    {
        if (ignoredInstances.TryGetValue(propertyType, out var funcs))
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

        if (ignoreEmptyCollections &&
            propertyType.IsCollection() ||
            propertyType.IsDictionary())
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