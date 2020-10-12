using System;
using System.Collections.Concurrent;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static Namer SharedNamer = new Namer();
        static ConcurrentDictionary<Type, Func<object, string>> parameterToNameLookup = new ConcurrentDictionary<Type, Func<object, string>>();

        public static void UniqueForRuntime()
        {
            SharedNamer.UniqueForRuntime = true;
        }

        public static void NameForParameter<T>(ParameterToName<T> func)
        {
            Guard.AgainstNull(func, nameof(func));
            parameterToNameLookup.AddOrUpdate(
                key: typeof(T),
                addValueFactory: _ => o => func((T) o),
                updateValueFactory: (_, _) => o => func((T) o));
        }

        internal static string GetNameForParameter(object parameter)
        {
            foreach (var parameterToName in parameterToNameLookup)
            {
                if (parameterToName.Key.IsInstanceOfType(parameter))
                {
                    return parameterToName.Value(parameter!);
                }
            }

            var nameForParameter = parameter!.ToString();
            if (nameForParameter == null)
            {
                throw new Exception($"{parameter.GetType().FullName} returned a null for `ToString()`.");
            }
            return nameForParameter;
        }

        public static void UniqueForAssemblyConfiguration()
        {
            SharedNamer.UniqueForAssemblyConfiguration = true;
        }

        public static void UniqueForRuntimeAndVersion()
        {
            SharedNamer.UniqueForRuntimeAndVersion = true;
        }
    }
}