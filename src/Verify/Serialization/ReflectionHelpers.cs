static class ReflectionHelpers
{
    public static bool InheritsFrom(this Type type, Type parent)
    {
        if (parent.IsAssignableFrom(type))
        {
            return true;
        }

        if (!parent.IsGenericTypeDefinition)
        {
            return false;
        }

        if (type.IsGeneric(parent))
        {
            return true;
        }

        if (parent.IsInterface)
        {
            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGeneric(parent))
                {
                    return true;
                }
            }

            return false;
        }

        var current = type;
        while (true)
        {
            if (current is null)
            {
                return false;
            }

            if (parent == current)
            {
                return true;
            }

            if (parent.IsGenericTypeDefinition &&
                current.IsGeneric(parent))
            {
                return true;
            }

            current = current.BaseType;
        }
    }

    public static Type MemberType(this MemberInfo member) =>
        member switch
        {
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
            _ => throw new($"No supported MemberType: {member.MemberType}")
        };

    public static bool IsEmptyCollectionOrDictionary(this object target) =>
        TryGetCollectionOrDictionary(target, out var isEmpty, out _) &&
        isEmpty.Value;

    public static bool TryGetCollectionOrDictionary(this object target, [NotNullWhen(true)] out bool? isEmpty, [NotNullWhen(true)] out IEnumerable? enumerable)
    {
        if (target is string)
        {
            enumerable = null;
            isEmpty = null;
            return false;
        }

        if (target is ICollection collection)
        {
            enumerable = collection;
            isEmpty = collection.Count == 0;
            return true;
        }

        if (target is not IEnumerable enumerableTarget)
        {
            enumerable = null;
            isEmpty = null;
            return false;
        }

        var type = target.GetType();

        if (type.IsEnumerableEmpty())
        {
            enumerable = enumerableTarget;
            isEmpty = true;
            return true;
        }

        if (type.ImplementsGenericCollection() ||
            type
                .GetInterfaces()
                .Any(ImplementsGenericCollection))
        {
            // ReSharper disable PossibleMultipleEnumeration
            enumerable = enumerableTarget;
            isEmpty = !enumerableTarget
                .GetEnumerator()
                .MoveNext();
            // ReSharper restore PossibleMultipleEnumeration
            return true;
        }

        enumerable = null;
        isEmpty = null;
        return false;
    }

    static bool IsEnumerableEmpty(this Type type) =>
        type.FullName?.StartsWith("System.Linq.EmptyPartition") == true;

    public static bool IsGeneric(this Type type, params Type[] generics)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var definition = type.GetGenericTypeDefinition();
        foreach (var generic in generics)
        {
            if (definition == generic)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsGeneric(this Type type, Type generic) =>
        type.IsGenericType &&
        type.GetGenericTypeDefinition() == generic;

    static bool ImplementsGenericCollection(this Type type) =>
        type.IsGeneric(
            typeof(ICollection<>),
            typeof(IReadOnlyCollection<>));
}