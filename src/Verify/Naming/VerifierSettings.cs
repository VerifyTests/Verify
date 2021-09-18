using System.Linq;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static Namer SharedNamer = new();
        static Dictionary<Type, Func<object, string>> parameterToNameLookup = new();

        public static void NameForParameter<T>(ParameterToName<T> func)
        {
            parameterToNameLookup.Add(typeof(T), o => func((T)o));
        }

        static char[] invalidPathChars =
        {
            '"',
            '\\',
            '<',
            '>',
            '|',
            '\u0000',
            '\u0001',
            '\u0002',
            '\u0003',
            '\u0004',
            '\u0005',
            '\u0006',
            '\u0007',
            '\b',
            '\t',
            '\n',
            '\u000b',
            '\f',
            '\r',
            '\u000e',
            '\u000f',
            '\u0010',
            '\u0011',
            '\u0012',
            '\u0013',
            '\u0014',
            '\u0015',
            '\u0016',
            '\u0017',
            '\u0018',
            '\u0019',
            '\u001a',
            '\u001b',
            '\u001c',
            '\u001d',
            '\u001e',
            '\u001f',
            ':',
            '*',
            '?',
            '/'
        };

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
            if (nameForParameter is null)
            {
                throw new($"{parameter.GetType().FullName} returned a null for `ToString()`.");
            }

            var builder = new StringBuilder();
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

        public static void UniqueForOSPlatform()
        {
            SharedNamer.UniqueForOSPlatform = true;
        }
    }
}