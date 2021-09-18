using System.Linq;

static class ReflectionHelpers
{
    public static bool IsDictionary(this Type type)
    {
        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            return true;
        }

        if (type.IsGenericDictionary())
        {
            return true;
        }

        return type.GetInterfaces().Any(IsGenericDictionary);
    }

    public static bool IsCollection(this Type type)
    {
        if (type.IsGenericList())
        {
            return true;
        }

        return type.GetInterfaces().Any(IsGenericList);
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

    static bool IsGenericList(this Type x)
    {
        if (!x.IsGenericType)
        {
            return false;
        }

        var definition = x.GetGenericTypeDefinition();
        return definition == typeof(ICollection<>) ||
               definition == typeof(IReadOnlyCollection<>);
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
}