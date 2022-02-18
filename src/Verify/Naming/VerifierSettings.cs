namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Namer SharedNamer = new();

    static Dictionary<Type, Func<object, string>> parameterToNameLookup = new()
    {
        {typeof(bool), target => ((bool) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(short), target => ((short) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(ushort), target => ((ushort) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(int), target => ((int) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(uint), target => ((uint) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(long), target => ((long) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(ulong), target => ((ulong) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(decimal), target => ((decimal) target).ToString(CultureInfo.InvariantCulture)},
#if NET5_0_OR_GREATER
        {typeof(Half), target => ((Half) target).ToString(CultureInfo.InvariantCulture)},
#endif
#if NET6_0_OR_GREATER
        {
            typeof(DateOnly), target =>
            {
                var date = (DateOnly) target;
                return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        },
        {
            typeof(TimeOnly), target =>
            {
                var time = (TimeOnly) target;
                return time.ToString("h-mm-tt", CultureInfo.InvariantCulture);
            }
        },
#endif
        {typeof(float), target => ((float) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(double), target => ((double) target).ToString(CultureInfo.InvariantCulture)},
        {
            typeof(DateTime),
            target =>
            {
                var dateTime = (DateTime) target;
                return dateTime.ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFFz");
            }
        },
        {
            typeof(DateTimeOffset),
            target =>
            {
                var dateTimeOffset = (DateTimeOffset) target;
                return dateTimeOffset.ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFFz", CultureInfo.InvariantCulture);
            }
        }
    };

    public static void NameForParameter<T>(ParameterToName<T> func)
    {
        parameterToNameLookup[typeof(T)] = o => func((T) o);
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

    internal static string GetNameForParameter(object? parameter)
    {
        if (parameter is null)
        {
            return "null";
        }

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

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public static void UniqueForRuntime()
    {
        SharedNamer.UniqueForRuntime = true;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public static void UniqueForTargetFramework()
    {
        SharedNamer.UniqueForTargetFramework = true;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public static void UniqueForTargetFrameworkAndVersion()
    {
        SharedNamer.UniqueForTargetFrameworkAndVersion = true;
    }

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public static void UniqueForAssemblyConfiguration()
    {
        SharedNamer.UniqueForAssemblyConfiguration = true;
    }

    /// <summary>
    /// Use <paramref name="assembly"/> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public static void UniqueForTargetFramework(Assembly assembly)
    {
        SharedNamer.UniqueForTargetFramework = true;
        SharedNamer.UniqueForTargetFrameworkAssembly = assembly;
    }

    /// <summary>
    /// Use the <paramref name="assembly"/> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public static void UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        SharedNamer.UniqueForTargetFrameworkAndVersion = true;
        SharedNamer.UniqueForTargetFrameworkAssembly = assembly;
    }

    /// <summary>
    /// Use the <paramref name="assembly"/> configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public static void UniqueForAssemblyConfiguration(Assembly assembly)
    {
        SharedNamer.UniqueForAssemblyConfiguration = true;
        SharedNamer.UniqueForAssemblyConfigurationAssembly = assembly;
    }

    /// <summary>
    /// Use the current runtime and runtime version to make the test results unique.
    /// Used when a test produces different results based on runtime and runtime version.
    /// </summary>
    public static void UniqueForRuntimeAndVersion()
    {
        SharedNamer.UniqueForRuntimeAndVersion = true;
    }

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    public static void UniqueForArchitecture()
    {
        SharedNamer.UniqueForArchitecture = true;
    }

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    public static void UniqueForOSPlatform()
    {
        SharedNamer.UniqueForOSPlatform = true;
    }

    internal static bool UniquePrefixDisabled;

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    public static void DisableRequireUniquePrefix()
    {
        UniquePrefixDisabled = true;
    }
}