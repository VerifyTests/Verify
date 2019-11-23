using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;

namespace ObjectApproval
{
    public static class TypeNameConverter
    {
        static ConcurrentDictionary<Type, string> cacheDictionary = new ConcurrentDictionary<Type, string>();

        static CSharpCodeProvider codeDomProvider = new CSharpCodeProvider();

        public static string GetName(Type type)
        {
            return cacheDictionary.GetOrAdd(type, Inner);
        }

        private static string Inner(Type type)
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

            var typeName = type.FullName.Replace(type.Namespace + ".", "");
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