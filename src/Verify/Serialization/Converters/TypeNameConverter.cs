namespace VerifyTests;

public static class TypeNameConverter
{
    static ConcurrentDictionary<Type, string> cacheDictionary = new(
        new List<KeyValuePair<Type, string>>
        {
            new(typeof(object), "object"),
            new(typeof(char), "char"),
            new(typeof(char?), "char?"),
            new(typeof(string), "string"),
            new(typeof(sbyte), "sbyte"),
            new(typeof(sbyte?), "sbyte?"),
            new(typeof(byte), "byte"),
            new(typeof(byte?), "byte?"),
            new(typeof(bool), "bool"),
            new(typeof(bool?), "bool?"),
            new(typeof(short), "short"),
            new(typeof(short?), "short?"),
            new(typeof(ushort), "ushort"),
            new(typeof(ushort?), "ushort?"),
            new(typeof(int), "int"),
            new(typeof(int?), "int?"),
            new(typeof(uint), "uint"),
            new(typeof(uint?), "uint?"),
            new(typeof(long), "long"),
            new(typeof(long?), "long?"),
            new(typeof(nint), "nint"),
            new(typeof(nint?), "nint"),
            new(typeof(nuint), "nuint"),
            new(typeof(nuint?), "nuint?"),
            new(typeof(decimal), "decimal"),
            new(typeof(decimal?), "decimal?"),
            new(typeof(float), "float"),
            new(typeof(float?), "float?"),
            new(typeof(double), "double"),
            new(typeof(double?), "double?"),
            new(typeof(Guid), "Guid"),
            new(typeof(Guid?), "Guid?"),
            new(typeof(DateTime), "DateTime"),
            new(typeof(DateTime?), "DateTime?"),
            new(typeof(DateTimeOffset), "DateTimeOffset"),
            new(typeof(DateTimeOffset?), "DateTimeOffset?"),
            new(typeof(TimeSpan), "TimeSpan"),
            new(typeof(TimeSpan?), "TimeSpan?"),
#if NET5_0_OR_GREATER
            new(typeof(Half), "Half"),
            new(typeof(Half?), "Half?"),
#endif
#if NET6_0_OR_GREATER
            new(typeof(DateOnly), "DateOnly"),
            new(typeof(DateOnly?), "DateOnly?"),
            new(typeof(TimeOnly), "TimeOnly"),
            new(typeof(TimeOnly?), "TimeOnly?"),
#endif
        });

    static ConcurrentDictionary<ICustomAttributeProvider, string> infoCache = new();

    public static string GetName(Type type)
    {
        return cacheDictionary.GetOrAdd(type, Inner);
    }

    public static string GetName(ParameterInfo parameter)
    {
        return infoCache.GetOrAdd(parameter, _ =>
        {
            var member = GetName(parameter.Member);
            var declaringType = GetName(parameter.Member.DeclaringType!);
            return $"{parameter.Name} of {declaringType}.{member}";
        });
    }

    public static string GetName(FieldInfo field)
    {
        return infoCache.GetOrAdd(field, _ =>
        {
            if (field.DeclaringType is null)
            {
                return $"Module.{field.Name}";
            }
            var declaringType = GetName(field.DeclaringType);
            return $"{declaringType}.{field.Name}";
        });
    }

    public static string GetName(PropertyInfo property)
    {
        return infoCache.GetOrAdd(property, _ =>
        {
            if (property.DeclaringType is null)
            {
                return $"Module.{property.Name}";
            }
            var declaringType = GetName(property.DeclaringType);
            return $"{declaringType}.{property.Name}";
        });
    }

    static string GetName(MemberInfo methodBase)
    {
        if (methodBase is ConstructorInfo constructor)
        {
            return GetName(constructor);
        }

        if (methodBase is MethodInfo method)
        {
            return GetName(method);
        }

        throw new InvalidOperationException();
    }

    public static string GetName(ConstructorInfo constructor)
    {
        return infoCache.GetOrAdd(constructor, _ =>
        {
            var declaringType = GetName(constructor.DeclaringType!);
            StringBuilder builder = new($"{declaringType}");
            if (constructor.IsStatic)
            {
                builder.Append(".cctor(");
            }
            else
            {
                builder.Append(".ctor(");
            }
            var parameters = constructor.GetParameters()
                .Select(x => $"{GetName(x.ParameterType)} {x.Name}");
            builder.Append(string.Join(", ", parameters));
            builder.Append(')');
            return builder.ToString();
        });
    }

    public static string GetName(MethodInfo method)
    {
        return infoCache.GetOrAdd(method, _ =>
        {
            var declaringType = GetName(method.DeclaringType!);
            StringBuilder builder = new($"{declaringType}.{method.Name}(");
            var parameters = method.GetParameters()
                .Select(x => $"{GetName(x.ParameterType)} {x.Name}");
            builder.Append(string.Join(", ", parameters));
            builder.Append(')');
            return builder.ToString();
        });
    }

    static string Inner(Type type)
    {
        if (IsAnonType(type))
        {
            return "dynamic";
        }

        if (type.Name.StartsWith("<") ||
            type.IsNested && type.DeclaringType == typeof(Enumerable))
        {
            var singleOrDefault = type.GetInterfaces()
                .SingleOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (singleOrDefault is not null)
            {
                if (singleOrDefault.GetGenericArguments().Single().IsAnonType())
                {
                    return "IEnumerable<dynamic>";
                }
                return GetName(singleOrDefault);
            }
        }

        if (typeof(IDictionaryWrapper).IsAssignableFrom(type))
        {
            //TODO: Should recurse here
            type = type.GetGenericArguments().Last();
        }

        return InnerGetName(type);
    }

    static string InnerGetName(Type type)
    {
        if (type.IsGenericParameter)
        {
            return type.Name;
        }
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            return $"{GetName(elementType)}[]";
        }

        //if (type.IsGenericTypeDefinition)
        //{
        //    var builder = new StringBuilder(GetName(type.GetGenericTypeDefinition()));
        //    builder.Append("<");
        //    foreach (var argument in type.GetGenericArguments())
        //    {
        //        builder.Append(GetName(argument) + ", ");
        //    }

        //    builder.Length -= 2;
        //    builder.Append(">");
        //    return builder.ToString();
        //}
        var typeName = type.Name;
        if (type.IsGenericType)
        {
            var tick = typeName.IndexOf('`');
            var builder = new StringBuilder(typeName.Substring(0, tick));
            builder.Append("<");
            foreach (var argument in type.GetGenericArguments())
            {
                builder.Append(GetName(argument) + ", ");
            }

            builder.Length -= 2;
            builder.Append(">");
            return builder.ToString();
        }
        
        if (type.IsNested)
        {
            return $"{GetName(type.DeclaringType!)}+{typeName}";
        }

        return typeName;
    }

    static bool IsAnonType(this Type type)
    {
        return type.Name.Contains("AnonymousType");
    }
}