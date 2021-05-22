using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static Namer SharedNamer = new();
        static Dictionary<Type, Func<object, string>> parameterToNameLookup = new();

        public static void NameForParameter<T>(ParameterToName<T> func)
        {
            Guard.AgainstNull(func, nameof(func));
            parameterToNameLookup.Add(typeof(T), o => func((T) o));
        }

        static char[] invalidPathChars = Path.GetInvalidPathChars()
            .Concat(Path.GetInvalidFileNameChars())
            .Distinct()
            .ToArray();

        internal static string GetNameForParameter(object parameter)
        {
            foreach (var parameterToName in parameterToNameLookup)
            {
                if (parameterToName.Key.IsInstanceOfType(parameter))
                {
                    return parameterToName.Value(parameter);
                }
            }

            var nameForParameter = parameter.ToString();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (nameForParameter != null)
            {
                StringBuilder builder = new();
                foreach (var ch in nameForParameter)
                {
                    if (invalidPathChars.Contains(ch))
                    {
                        builder.Append('-');
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
                return builder.ToString();
            }

            throw new($"{parameter.GetType().FullName} returned a null for `ToString()`.");
        }

        public static void UniqueForRuntime()
        {
            SharedNamer.UniqueForRuntime = true;
        }

        public static void UniqueForAssemblyConfiguration()
        {
            SharedNamer.UniqueForAssemblyConfiguration = true;
        }

        public static void UniqueForRuntimeAndVersion()
        {
            SharedNamer.UniqueForRuntimeAndVersion = true;
        }

        public static void UniqueForArchitecture()
        {
            SharedNamer.UniqueForArchitecture = true;
        }
    }
}