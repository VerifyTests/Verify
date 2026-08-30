using System.Collections.Immutable;

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
        target.TryGetCollectionOrDictionary(out var isEmpty, out _) &&
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
            if (IsDefaultOrEmptyImmutableArray(target))
            {
                enumerable = Array.Empty<object>();
                isEmpty = true;
                return true;
            }

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

        switch (enumerableKinds.GetOrAdd(target.GetType(), GetEnumerableKind))
        {
            case EnumerableKind.AlwaysEmpty:
                enumerable = enumerableTarget;
                isEmpty = true;
                return true;
            case EnumerableKind.Enumerate:
                enumerable = enumerableTarget;
                isEmpty = IsEmpty(enumerableTarget);
                return true;
            default:
                enumerable = null;
                isEmpty = null;
                return false;
        }
    }

    enum EnumerableKind
    {
        NotACollection,
        AlwaysEmpty,
        Enumerate
    }

    /// <summary>
    /// Whether a value is a collection depends only on its type, but the answer used to be
    /// recomputed for every value. With the default ignoreEmptyCollections this runs for
    /// every non null member value, array item and dictionary value, and for anything that
    /// is not a non generic ICollection (every HashSet, dictionary key and value view,
    /// iterator, LINQ result and immutable collection) it allocated a fresh Type[] from
    /// GetInterfaces plus several LINQ passes over it. Only the per value IsEmpty
    /// enumeration is left.
    /// </summary>
    static ConcurrentDictionary<Type, EnumerableKind> enumerableKinds = new();

    static EnumerableKind GetEnumerableKind(Type type)
    {
        if (type.IsEnumerableEmpty())
        {
            return EnumerableKind.AlwaysEmpty;
        }

        if (IsLookup(type))
        {
            return EnumerableKind.Enumerate;
        }

        var interfaces = type.GetInterfaces();

        if (interfaces.Any(IsLookup))
        {
            return EnumerableKind.Enumerate;
        }

        if (type.ImplementsGenericCollection() ||
            interfaces.Any(ImplementsGenericCollection))
        {
            return EnumerableKind.Enumerate;
        }

        return EnumerableKind.NotACollection;
    }

    static bool IsLookup(Type type) =>
        type.FullName?.StartsWith("System.Linq.ILookup", StringComparison.Ordinal) == true;

    static bool IsEmpty(IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        using var disposable = enumerator as IDisposable;
        return !enumerator.MoveNext();
    }

    static bool IsEnumerableEmpty(this Type type) =>
        type.FullName?.StartsWith("System.Linq.EmptyPartition", StringComparison.Ordinal) == true;

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

    static ConcurrentDictionary<Type, MethodInfo?> isDefaultOrEmptyGetters = new();

    static bool IsDefaultOrEmptyImmutableArray(object target)
    {
        var getter = isDefaultOrEmptyGetters.GetOrAdd(target.GetType(), BuildIsDefaultOrEmptyGetter);
        return getter != null &&
               (bool)getter.Invoke(target, null)!;
    }

    static MethodInfo? BuildIsDefaultOrEmptyGetter(Type targetType)
    {
        if (!targetType.IsGeneric(typeof(ImmutableArray<>)))
        {
            return null;
        }

        var isDefaultOrEmptyProperty = targetType.GetProperty(
                                           name: nameof(ImmutableArray<>.IsDefaultOrEmpty),
                                           bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                                       ?? throw new NotSupportedException("There is no IsDefaultOrEmpty property on ImmutableArray.");

        return isDefaultOrEmptyProperty.GetMethod!;
    }
}
