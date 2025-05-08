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
#if NET6_0_OR_GREATER
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
        parameterToNameLookup[typeof(T)] = _ => func((T) _);
    }

    public static string GetNameForParameter(object? parameter) =>
        GetNameForParameter(parameter, true);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public static string GetNameForParameter(object? parameter, bool pathFriendly = true) =>
        GetNameForParameter(parameter, null, pathFriendly);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public static string GetNameForParameter(object? parameter, Counter? counter = null, bool pathFriendly = true)
    {
        if (parameter is null)
        {
            return "null";
        }

        var builder = new StringBuilder();
#pragma warning disable AppendParameter
        AppendParameter(parameter, builder, true, counter ?? Counter.CurrentOrNull, pathFriendly);
#pragma warning restore AppendParameter
        return builder.ToString();
    }

    [Experimental("AppendParameter")]
    public static void AppendParameter(object? parameter, StringBuilder builder, bool isRoot, Counter? counter, bool pathFriendly = true)
    {
        while (true)
        {
            if (parameter is null)
            {
                builder.Append("null");
                return;
            }

            if (counter != null && counter.TryGetNamed(parameter, out var result))
            {
                builder.Append(result);
                return;
            }

            if (parameter is string stringParameter)
            {
                if (pathFriendly)
                {
                    FileNameCleaner.AppendValid(builder, stringParameter);
                }
                else
                {
                    builder.Append(stringParameter);
                }

                return;
            }

            var type = parameter.GetType();

            if (TryGetParameterToNameLookup(type, out var lookup))
            {
                builder.Append(lookup(parameter));
                return;
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
                    AppendParameter(item, builder, false, counter);
                    builder.Append(',');
                }

                builder.Length--;

                if (!isRoot)
                {
                    builder.Append(']');
                }

                return;
            }

            if (TryGetKeyValue(type, parameter, out var key, out var value))
            {
                AppendParameter(key, builder, true, counter, pathFriendly);
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

            if (pathFriendly)
            {
                FileNameCleaner.AppendValid(builder, nameForParameter);
            }
            else
            {
                builder.Append(nameForParameter);
            }

            break;
        }
    }

    static bool TryGetKeyValue(Type type, object target, out object? key, out object? value)
    {
        if (!type.IsGeneric(typeof(KeyValuePair<,>)))
        {
            key = null;
            value = null;
            return false;
        }

#if NETFRAMEWORK
        key = type.GetProperty("Key")!.GetMethod!.Invoke(target, null);
        value = type.GetProperty("Value")!.GetMethod!.Invoke(target, null);
#else
        var parameters = new object?[] { null, null };
        type.GetMethod("Deconstruct")!.Invoke(target, parameters);
        key = parameters[0];
        value = parameters[1];
#endif

        return true;
    }

    static bool TryGetParameterToNameLookup(Type type, [NotNullWhen(true)] out Func<object, string>? lookup)
    {
        if (parameterToNameLookup.TryGetValue(type, out lookup))
        {
            return true;
        }

        foreach (var (key, value) in parameterToNameLookup)
        {
            if (key.IsAssignableFrom(type))
            {
                lookup = value;
                return true;
            }
        }

        return false;
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