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

        if (type.IsGenericType && type.GetGenericTypeDefinition() == parent)
        {
            return true;
        }

        if (parent.IsInterface)
        {
            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType)
                {
                    if (@interface.GetGenericTypeDefinition() == parent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        Type? current = type;
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

            if (parent.IsGenericTypeDefinition && current.IsGenericType)
            {
                if (current.GetGenericTypeDefinition() == parent)
                {
                    return true;
                }
            }

            current = current.BaseType;
        }
    }

    public static Type MemberType(this MemberInfo member)
    {
        if (member is PropertyInfo property)
        {
            return property.PropertyType;
        }

        if (member is FieldInfo field)
        {
            return field.FieldType;
        }

        throw new($"No supported MemberType: {member.MemberType}");
    }

    public static bool IsEmptyCollectionOrDictionary(this object target)
    {
        if (target is string)
        {
            return false;
        }

        if (target is ICollection collection)
        {
            if (collection.Count == 0)
            {
                return true;
            }

            return false;
        }

        if (target is not IEnumerable enumerable)
        {
            return false;
        }

        var type = target.GetType();

        if (type.IsEnumerableEmpty())
        {
            return true;
        }

        if (type.IsGenericCollection())
        {
            var enumerator = enumerable.GetEnumerator();
            return !enumerator.MoveNext();
        }

        return false;
    }

    static bool IsEnumerableEmpty(this Type type) =>
        type.FullName?.StartsWith("System.Linq.EmptyPartition") == true;

    static bool IsGenericCollection(this Type type)
    {
        if (type.ImplementsGenericCollection())
        {
            return true;
        }

        var interfaces = type.GetInterfaces();

        if (interfaces.Any(ImplementsGenericCollection))
        {
            return true;
        }

        return false;
    }

    static bool ImplementsGenericCollection(this Type x)
    {
        if (!x.IsGenericType)
        {
            return false;
        }

        var definition = x.GetGenericTypeDefinition();
        return definition == typeof(ICollection<>) ||
               definition == typeof(IReadOnlyCollection<>);
    }
}