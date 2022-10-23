namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static Namer SharedNamer = new();

    static Dictionary<Type, Func<VerifySettings, object, string>> parameterToNameLookup = new()
    {
        {typeof(bool), (_, value) => ((bool) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(short), (_, value) => ((short) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(ushort), (_, value) => ((ushort) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(int), (_, value) => ((int) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(uint), (_, value) => ((uint) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(long), (_, value) => ((long) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(ulong), (_, value) => ((ulong) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(decimal), (_, value) => ((decimal) value).ToString(CultureInfo.InvariantCulture)},
#if NET5_0_OR_GREATER
        {typeof(Half), (_, value) => ((Half) value).ToString(CultureInfo.InvariantCulture)},
#endif
#if NET6_0_OR_GREATER
        {typeof(DateOnly), (_, value) => ((DateOnly) value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},
        {typeof(TimeOnly), (_, value) => ((TimeOnly) value).ToString("h-mm-tt", CultureInfo.InvariantCulture)},
#endif
        {typeof(float), (_, value) => ((float) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(double), (_, value) => ((double) value).ToString(CultureInfo.InvariantCulture)},
        {typeof(DateTime), (_, value) => DateFormatter.ToParameterString((DateTime) value)},
        {typeof(DateTimeOffset), (_, value) => DateFormatter.ToParameterString((DateTimeOffset) value)}
    };

    public static void NameForParameter<T>(ParameterToName<T> func) =>
        parameterToNameLookup[typeof(T)] = (settings, value) => func(settings, (T) value);

    internal static string GetNameForParameter(VerifySettings settings, object? parameter)
    {
        var builder = new StringBuilder();
        GetNameForParameter(settings, parameter, builder, true);
        return builder.ToString();
    }

    static void GetNameForParameter(VerifySettings settings, object? parameter, StringBuilder builder, bool isRoot)
    {
        if (parameter is null)
        {
            builder.Append("null");
            return;
        }

        foreach (var parameterToName in parameterToNameLookup)
        {
            if (parameterToName.Key.IsInstanceOfType(parameter))
            {
                builder.Append(parameterToName.Value(settings, parameter));
                return;
            }
        }

        if (parameter is string stringParameter)
        {
            builder.Append(stringParameter.ReplaceInvalidFileNameChars());
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
                GetNameForParameter(settings, item, builder, false);
                builder.Append(',');
            }

            builder.Length--;

            if (!isRoot)
            {
                builder.Append(']');
            }
            return;
        }

        var type = parameter.GetType();
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyMember = type.GetProperty("Key")!.GetMethod!.Invoke(parameter, null);
                var valueMember = type.GetProperty("Value")!.GetMethod!.Invoke(parameter, null);
                builder.Append($"{GetNameForParameter(settings, keyMember)}={GetNameForParameter(settings, valueMember)}");
                return;
            }
        }

        var nameForParameter = parameter.ToString();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (nameForParameter is null)
        {
            throw new($"{type.FullName} returned a null for `ToString()`.");
        }

        builder.Append(nameForParameter.ReplaceInvalidFileNameChars());
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