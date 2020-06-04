using System;
using System.Collections.Generic;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        internal static Namer SharedNamer = new Namer();
        static Dictionary<Type, Func<object, string>> parameterToNameLookup = new Dictionary<Type, Func<object, string>>();

        public static void UniqueForRuntime()
        {
            SharedNamer.UniqueForRuntime = true;
        }

        public static void NameForParameter<T>(ParameterToName<T> func)
        {
            Guard.AgainstNull(func, nameof(func));
            parameterToNameLookup.Add(typeof(T), o => func((T)o));
        }

        public static string GetNameForParameter(object parameter)
        {
            foreach (var parameterToName in parameterToNameLookup)
            {
                if (parameterToName.Key.IsInstanceOfType(parameter))
                {
                    return parameterToName.Value(parameter!);
                }
            }

            return parameter!.ToString();
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