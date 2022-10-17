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
}