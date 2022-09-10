﻿namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Namer SharedNamer = new();

    static Dictionary<Type, Func<object, string>> parameterToNameLookup = new()
    {
        {typeof(bool), _ => ((bool) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(short), _ => ((short) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(ushort), _ => ((ushort) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(int), _ => ((int) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(uint), _ => ((uint) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(long), _ => ((long) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(ulong), _ => ((ulong) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(decimal), _ => ((decimal) _).ToString(CultureInfo.InvariantCulture)},
#if NET5_0_OR_GREATER
        {typeof(Half), _ => ((Half) _).ToString(CultureInfo.InvariantCulture)},
#endif
#if NET6_0_OR_GREATER
        {typeof(DateOnly), _ => ((DateOnly) _).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},
        {typeof(TimeOnly), _ => ((TimeOnly) _).ToString("h-mm-tt", CultureInfo.InvariantCulture)},
#endif
        {typeof(float), _ => ((float) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(double), _ => ((double) _).ToString(CultureInfo.InvariantCulture)},
        {typeof(DateTime), _ => ((DateTime) _).ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFFz")},
        {typeof(DateTimeOffset), _ => ((DateTimeOffset) _).ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFFz", CultureInfo.InvariantCulture)}
    };

    public static void NameForParameter<T>(ParameterToName<T> func) =>
        parameterToNameLookup[typeof(T)] = o => func((T) o);

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

        if (parameter is IEnumerable enumerable &&
            parameter.GetType().IsCollection())
        {
            var innerBuilder = new StringBuilder();
            foreach (var item in enumerable)
            {
                innerBuilder.Append(GetNameForParameter(item));
                innerBuilder.Append(',');
            }
            innerBuilder.Length--;

            return innerBuilder.ToString();
        }

        var nameForParameter = parameter.ToString();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (nameForParameter is null)
        {
            throw new($"{parameter.GetType().FullName} returned a null for `ToString()`.");
        }

        return nameForParameter.ReplaceInvalidPathChars();
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public static void UniqueForRuntime() =>
        SharedNamer.UniqueForRuntime = true;

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public static void UseSplitModeForUniqueDirectory() =>
        UseUniqueDirectorySplitMode = true;

    internal static bool UseUniqueDirectorySplitMode;

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public static void UniqueForTargetFramework() =>
        SharedNamer.UniqueForTargetFramework = true;

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public static void UniqueForTargetFrameworkAndVersion() =>
        SharedNamer.UniqueForTargetFrameworkAndVersion = true;

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public static void UniqueForAssemblyConfiguration() =>
        SharedNamer.UniqueForAssemblyConfiguration = true;

    /// <summary>
    /// Use <paramref name="assembly" /> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public static void UniqueForTargetFramework(Assembly assembly)
    {
        SharedNamer.UniqueForTargetFramework = true;
        SharedNamer.UniqueForTargetFrameworkAssembly = assembly;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public static void UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        SharedNamer.UniqueForTargetFrameworkAndVersion = true;
        SharedNamer.UniqueForTargetFrameworkAssembly = assembly;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> configuration (debug/release) to make the test results unique.
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
    public static void UniqueForRuntimeAndVersion() =>
        SharedNamer.UniqueForRuntimeAndVersion = true;

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    public static void UniqueForArchitecture() =>
        SharedNamer.UniqueForArchitecture = true;

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    public static void UniqueForOSPlatform() =>
        SharedNamer.UniqueForOSPlatform = true;

    internal static bool UniquePrefixDisabled;

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    public static void DisableRequireUniquePrefix() =>
        UniquePrefixDisabled = true;
}