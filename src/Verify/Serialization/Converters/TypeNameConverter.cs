using System.CodeDom;
using Microsoft.CSharp;

namespace VerifyTests;

public static class TypeNameConverter
{
    static ConcurrentDictionary<Type, string> cacheDictionary = new(
        new List<KeyValuePair<Type, string>>
        {
            new(typeof(char), "Char"),
            new(typeof(char?), "Nullable<Char>"),
            new(typeof(string), "String"),
            new(typeof(sbyte), "SByte"),
            new(typeof(sbyte?), "Nullable<SByte>"),
            new(typeof(byte), "Byte"),
            new(typeof(byte?), "Nullable<Byte>"),
            new(typeof(bool), "Boolean"),
            new(typeof(bool?), "Nullable<Boolean>"),
            new(typeof(short), "Int16"),
            new(typeof(short?), "Nullable<Int16>"),
            new(typeof(ushort), "UInt16"),
            new(typeof(ushort?), "Nullable<UInt16>"),
            new(typeof(int), "Int32"),
            new(typeof(int?), "Nullable<Int32>"),
            new(typeof(uint), "UInt32"),
            new(typeof(uint?), "Nullable<UInt32>"),
            new(typeof(long), "Int64"),
            new(typeof(long?), "Nullable<Int64>"),
            new(typeof(nint), "IntPtr"),
            new(typeof(nint?), "Nullable<IntPtr>"),
            new(typeof(nuint), "UIntPtr"),
            new(typeof(nuint?), "Nullable<UIntPtr>"),
            new(typeof(decimal), "Decimal"),
            new(typeof(decimal?), "Nullable<Decimal>"),
            new(typeof(float), "Single"),
            new(typeof(float?), "Nullable<Single>"),
            new(typeof(double), "Double"),
            new(typeof(double?), "Nullable<Double>"),
            new(typeof(Guid), "Guid"),
            new(typeof(Guid?), "Nullable<Guid>"),
            new(typeof(DateTime), "DateTime"),
            new(typeof(DateTime?), "Nullable<DateTime>"),
            new(typeof(DateTimeOffset), "DateTimeOffset"),
            new(typeof(DateTimeOffset?), "Nullable<DateTimeOffset>"),
            new(typeof(TimeSpan), "TimeSpan"),
            new(typeof(TimeSpan?), "Nullable<TimeSpan>"),
#if NET5_0_OR_GREATER
            new(typeof(Half), "Half"),
            new(typeof(Half?), "Nullable<Half>"),
#endif
#if NET6_0_OR_GREATER
                new(typeof(DateOnly), "DateOnly"),
                new(typeof(DateOnly?), "Nullable<DateOnly>"),
                new(typeof(TimeOnly), "TimeOnly"),
                new(typeof(TimeOnly?), "Nullable<TimeOnly>"),
#endif
        });

    static ConcurrentDictionary<ICustomAttributeProvider, string> infoCache = new();

    static CSharpCodeProvider codeDomProvider = new();

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
            type = type.GetGenericArguments().Last();
        }

        var typeName = GetTypeName(type);
        CodeTypeReference reference = new(typeName);
        var name = codeDomProvider.GetTypeOutput(reference);
        var list = new List<string>();
        AllGenericArgumentNamespace(type, list);
        foreach (var ns in list.Distinct())
        {
            name = name.Replace($"<{ns}.", "<");
            name = name.Replace($", {ns}.", ", ");
        }

        return name;
    }

    static string GetTypeName(Type type)
    {
        if (type.FullName is null)
        {
            return type.Name;
        }
        return type.FullName.Replace(type.Namespace + ".", "");
    }

    static bool IsAnonType(this Type type)
    {
        return type.Name.Contains("AnonymousType");
    }

    static void AllGenericArgumentNamespace(Type type, List<string> list)
    {
        if (type.Namespace is not null)
        {
            list.Add(type.Namespace);
        }

        var elementType = type.GetElementType();

        if (elementType is not null)
        {
            AllGenericArgumentNamespace(elementType,list);
        }

        foreach (var generic in type.GenericTypeArguments)
        {
            AllGenericArgumentNamespace(generic, list);
        }
    }
}