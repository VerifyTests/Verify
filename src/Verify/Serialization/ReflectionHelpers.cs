static class ReflectionHelpers
{
    public static bool IsDictionary(this Type type)
    {
        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            return true;
        }

        if (!type.ImplementsIEnumerable())
        {
            return false;
        }

        if (type.IsGenericDictionary())
        {
            return true;
        }

        return type.GetInterfaces().Any(IsGenericDictionary);
    }

    static bool IsGenericDictionary(this Type x)
    {
        if (!x.IsGenericType)
        {
            return false;
        }

        var definition = x.GetGenericTypeDefinition();
        return definition == typeof(IDictionary<,>) ||
               definition == typeof(IReadOnlyDictionary<,>);
    }

    public static bool HasMembers(this IEnumerable? collection)
    {
        if (collection is null)
        {
            // if the list is null, we defer the decision to NullValueHandling
            return true;
        }

        // check to see if there is at least one item in the Enumerable
        return collection.GetEnumerator().MoveNext();
    }

    static bool ImplementsICollection(this Type interfaceType)
    {
        return typeof(ICollection).IsAssignableFrom(interfaceType);
    }

    static bool ImplementsIEnumerable(this Type interfaceType)
    {
        return typeof(IEnumerable).IsAssignableFrom(interfaceType);
    }

    public static bool IsCollectionOrDictionary(this Type type)
    {
        return type.IsCollection() ||
               type.IsDictionary();
    }

    public static bool IsCollection(this Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        if (type.ImplementsICollection())
        {
            return true;
        }

        if (!type.ImplementsIEnumerable())
        {
            return false;
        }

        if (type.IsGenericCollection())
        {
            return true;
        }

        var interfaces = type.GetInterfaces();

        if (interfaces.Any(ImplementsICollection))
        {
            return true;
        }

        if (interfaces.Any(IsGenericCollection))
        {
            return true;
        }

        return false;
    }

    static bool IsGenericCollection(this Type x)
    {
        if (!x.IsGenericType)
        {
            return false;
        }

        var definition = x.GetGenericTypeDefinition();
        return definition == typeof(ICollection<>) ||
               definition == typeof(IReadOnlyCollection<>);
    }


    public static bool ImplementsStreamEnumerable(this Type type)
    {
        return type.GetInterfaces()
            .Any(_ => _.IsStreamEnumerable());
    }

    static bool IsStreamEnumerable(this Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        if (type.GetGenericTypeDefinition() != typeof(IEnumerable<>))
        {
            return false;
        }

        return type.GetGenericArguments()[0] == typeof(Stream);
    }

    public static T GetValue<T>(this MemberInfo member, object instance)
    {
        // this value could be in a public field or public property
        if (member is PropertyInfo propertyInfo)
        {
            return (T) propertyInfo.GetValue(instance, null)!;
        }

        if (member is FieldInfo fieldInfo)
        {
            return (T) fieldInfo.GetValue(instance)!;
        }

        throw new($"No supported MemberType: {member.MemberType}");
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
}