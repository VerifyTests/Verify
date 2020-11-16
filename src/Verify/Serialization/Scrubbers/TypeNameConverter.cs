using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace VerifyTests
{
    public static class TypeNameConverter
    {
        static Dictionary<Type, string> cacheDictionary = new();
        static Dictionary<ICustomAttributeProvider, string> infoCache = new();

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
                if (field.DeclaringType == null)
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
                if (property.DeclaringType == null)
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
                var builder = new StringBuilder($"{declaringType}");
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
                var builder = new StringBuilder($"{declaringType}.{method.Name}(");
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

            if (type.Name.StartsWith("<")
                || type.IsNested && type.DeclaringType == typeof(Enumerable))
            {
                var singleOrDefault = type.GetInterfaces()
                    .SingleOrDefault(x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (singleOrDefault != null)
                {
                    if (singleOrDefault.GetGenericArguments().Single().IsAnonType())
                    {
                        return "IEnumerable<dynamic>";
                    }
                    return GetName(singleOrDefault);
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DictionaryWrapper<,>))
            {
                type = type.GetGenericArguments().Last();
            }

            var typeName = GetTypeName(type);
            var reference = new CodeTypeReference(typeName);
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
            if (type.FullName == null)
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
            if (type.Namespace != null)
            {
                list.Add(type.Namespace);
            }

            var elementType = type.GetElementType();

            if (elementType != null)
            {
                AllGenericArgumentNamespace(elementType,list);
            }

            foreach (var generic in type.GenericTypeArguments)
            {
                AllGenericArgumentNamespace(generic, list);
            }
        }
    }
}