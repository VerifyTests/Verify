namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Namer SharedNamer = new();

    static Dictionary<Type, Func<object, string>> parameterToNameLookup = new()
    {
        {
            typeof(bool), _ => ((bool) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(short), _ => ((short) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(ushort), _ => ((ushort) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(int), _ => ((int) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(uint), _ => ((uint) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(long), _ => ((long) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(ulong), _ => ((ulong) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(decimal), _ => ((decimal) _).ToString(Culture.InvariantCulture)
        },
#if NET5_0_OR_GREATER
        {
            typeof(Half), _ => ((Half) _).ToString(Culture.InvariantCulture)
        },
#endif
#if NET6_0_OR_GREATER
        {
            typeof(Date), _ => ((Date) _).ToString("yyyy-MM-dd", Culture.InvariantCulture)
        },
        {
            typeof(Time), _ => ((Time) _).ToString("h-mm-tt", Culture.InvariantCulture)
        },
#endif
        {
            typeof(float), _ => ((float) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(double), _ => ((double) _).ToString(Culture.InvariantCulture)
        },
        {
            typeof(DateTime), _ => DateFormatter.ToParameterString((DateTime) _)
        },
        {
            typeof(DateTimeOffset), _ => DateFormatter.ToParameterString((DateTimeOffset) _)
        }
    };

    public static void NameForParameter<T>(ParameterToName<T> func)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        parameterToNameLookup[typeof(T)] = o => func((T) o);
    }

    public static string GetNameForParameter(object? parameter)
    {
        var builder = new StringBuilder();
        AppendParameter(parameter, builder, true);
        return builder.ToString();
    }

    internal static void AppendParameter(object? parameter, StringBuilder builder, bool isRoot)
    {
        while (true)
        {
            if (parameter is null)
            {
                builder.Append("null");
                return;
            }

            if (parameter is string stringParameter)
            {
                FileNameCleaner.AppendValid(builder, stringParameter);
                return;
            }

            var type = parameter.GetType();

            if (parameterToNameLookup.TryGetValue(type, out var lookup))
            {
                builder.Append(lookup(parameter));
                return;
            }

            foreach (var (key, value) in parameterToNameLookup)
            {
                if (key.IsAssignableFrom(type))
                {
                    builder.Append(value(parameter));
                    return;
                }
            }

            if (parameter.TryGetCollectionOrDictionary(out var isEmpty, out var enumerable))
            {
                if (isEmpty.Value)
                {
                    builder.Append("[]");
                    return;
                }

                if (!isRoot)
                {
                    builder.Append('[');
                }

                foreach (var item in enumerable)
                {
                    AppendParameter(item, builder, false);
                    builder.Append(',');
                }

                builder.Length--;

                if (!isRoot)
                {
                    builder.Append(']');
                }

                return;
            }

            if (type.IsGeneric(typeof(KeyValuePair<,>)))
            {
                var key = type.GetProperty("Key")!.GetMethod!.Invoke(parameter, null);
                var value = type.GetProperty("Value")!.GetMethod!.Invoke(parameter, null);
                AppendParameter(key, builder, true);
                builder.Append('=');
                parameter = value;
                isRoot = true;
                continue;
            }

            var nameForParameter = parameter.ToString();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (nameForParameter is null)
            {
                throw new($"{type.FullName} returned a null for `ToString()`.");
            }

            FileNameCleaner.AppendValid(builder, nameForParameter);
            break;
        }
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public static void UniqueForRuntime()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForRuntime = true;
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public static void UseSplitModeForUniqueDirectory()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        UseUniqueDirectorySplitMode = true;
    }

    internal static bool UseUniqueDirectorySplitMode;

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public static void UniqueForTargetFramework()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForTargetFramework = true;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public static void UniqueForTargetFrameworkAndVersion()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForTargetFrameworkAndVersion = true;
    }

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public static void UniqueForAssemblyConfiguration()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForAssemblyConfiguration = true;
    }

    /// <summary>
    /// Use <paramref name="assembly" /> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public static void UniqueForTargetFramework(Assembly assembly)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForTargetFramework = true;
        SharedNamer.SetUniqueForAssemblyFrameworkName(assembly);
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public static void UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForTargetFrameworkAndVersion = true;
        SharedNamer.SetUniqueForAssemblyFrameworkName(assembly);
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public static void UniqueForAssemblyConfiguration(Assembly assembly)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForAssemblyConfiguration = true;
        SharedNamer.SetUniqueForAssemblyConfiguration(assembly);
    }

    /// <summary>
    /// Use the current runtime and runtime version to make the test results unique.
    /// Used when a test produces different results based on runtime and runtime version.
    /// </summary>
    public static void UniqueForRuntimeAndVersion()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForRuntimeAndVersion = true;
    }

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    public static void UniqueForArchitecture()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForArchitecture = true;
    }

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    public static void UniqueForOSPlatform()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        SharedNamer.UniqueForOSPlatform = true;
    }

    internal static bool UniquePrefixDisabled;

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    public static void DisableRequireUniquePrefix()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        UniquePrefixDisabled = true;
    }
}