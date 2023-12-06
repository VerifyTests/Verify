namespace VerifyTests;

public class Namer
{
    static string? assemblyConfig;

    public static string AssemblyConfig
    {
        get
        {
            if (assemblyConfig is not null)
            {
                return assemblyConfig;
            }

            throw new("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
        }
    }

    internal static void UseAssembly(Assembly assembly)
    {
        assemblyConfig = assembly.Configuration();

        var frameworkName = assembly.FrameworkName();

        if (frameworkName is not null)
        {
            targetFrameworkName = frameworkName.Name;
            targetFrameworkNameAndVersion = frameworkName.NameAndVersion;
        }
    }

    public static string GetSimpleFrameworkName(FrameworkName name)
    {
        var identifier = name.Identifier;

        if (identifier.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
        {
            return "Net";
        }

        if (identifier.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase))
        {
            return "Net";
        }

        if (identifier.StartsWith(".NET", StringComparison.OrdinalIgnoreCase))
        {
            return "DotNet";
        }

        throw new($"Could not resolve runtime for '{identifier}'.");
    }

    internal bool UniqueForRuntime;

    internal bool ResolveUniqueForRuntime() =>
        UniqueForRuntime ||
        VerifierSettings.SharedNamer.UniqueForRuntime;

    internal bool UniqueForTargetFramework;

    internal bool ResolveUniqueForTargetFramework() =>
        UniqueForTargetFramework ||
        VerifierSettings.SharedNamer.UniqueForTargetFramework;

    internal FrameworkNameVersion? UniqueForTargetFrameworkName;

    internal void SetUniqueForAssemblyFrameworkName(Assembly assembly) =>
        UniqueForTargetFrameworkName = assembly.FrameworkName() ??
                                       throw new($"UniqueForTargetFrameworkAndVersion used but no `TargetFrameworkAttribute` found in {assembly.FullName}.");

    internal FrameworkNameVersion? ResolveUniqueForTargetFrameworkName() =>
        UniqueForTargetFrameworkName ??
        VerifierSettings.SharedNamer.UniqueForTargetFrameworkName;

    internal bool UniqueForAssemblyConfiguration;

    internal bool ResolveUniqueForAssemblyConfiguration() =>
        UniqueForAssemblyConfiguration ||
        VerifierSettings.SharedNamer.UniqueForAssemblyConfiguration;

    internal string? UniqueForAssemblyConfigurationValue;

    internal void SetUniqueForAssemblyConfiguration(Assembly assembly) =>
        UniqueForAssemblyConfigurationValue = assembly.Configuration() ??
                                              throw new($"UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found in {assembly.FullName}.");

    internal string? ResolveUniqueForAssemblyConfigurationValue() =>
        UniqueForAssemblyConfigurationValue ??
        VerifierSettings.SharedNamer.UniqueForAssemblyConfigurationValue;

    internal bool UniqueForRuntimeAndVersion;

    internal bool ResolveUniqueForRuntimeAndVersion() =>
        UniqueForRuntimeAndVersion ||
        VerifierSettings.SharedNamer.UniqueForRuntimeAndVersion;

    internal bool UniqueForTargetFrameworkAndVersion;

    internal bool ResolveUniqueForTargetFrameworkAndVersion() =>
        UniqueForTargetFrameworkAndVersion ||
        VerifierSettings.SharedNamer.UniqueForTargetFrameworkAndVersion;

    internal bool UniqueForArchitecture;

    internal bool ResolveUniqueForArchitecture() =>
        UniqueForArchitecture ||
        VerifierSettings.SharedNamer.UniqueForArchitecture;

    internal bool UniqueForOSPlatform;

    internal bool ResolveUniqueForOSPlatform() =>
        UniqueForOSPlatform ||
        VerifierSettings.SharedNamer.UniqueForOSPlatform;

    static string? targetFrameworkName;
    static string? targetFrameworkNameAndVersion;

    public static string TargetFrameworkNameAndVersion
    {
        get
        {
            if (targetFrameworkNameAndVersion is not null)
            {
                return targetFrameworkNameAndVersion;
            }

            throw new("UniqueForTargetFrameworkAndVersion or UniqueForTargetFramework used but no `TargetFrameworkAttribute` found.");
        }
    }

    public static string TargetFrameworkName
    {
        get
        {
            if (targetFrameworkName is not null)
            {
                return targetFrameworkName;
            }

            throw new("UniqueForTargetFrameworkAndVersion or UniqueForTargetFramework used but no `TargetFrameworkAttribute` found.");
        }
    }

    static Namer()
    {
        var (runtime, version) = GetRuntimeAndVersion();
        Runtime = runtime;
        RuntimeAndVersion = $"{runtime}{version.Major}_{version.Minor}";
        Architecture = RuntimeInformation
            .ProcessArchitecture.ToString()
            .ToLower();
        OperatingSystemPlatform = GetOSPlatform();
    }

    static string GetOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "Linux";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "Windows";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "OSX";
        }

#if NET5_0_OR_GREATER

        if (OperatingSystem.IsAndroid())
        {
            return "Android";
        }

        if (OperatingSystem.IsIOS())
        {
            return "IOS";
        }

#endif

        throw new("Unknown OS");
    }

    public static string Runtime { get; }

    public static string RuntimeAndVersion { get; }

    public static string Architecture { get; }

    public static string OperatingSystemPlatform { get; }

    internal Namer()
    {
    }

    internal Namer(Namer namer)
    {
        UniqueForRuntime = namer.UniqueForRuntime;
        UniqueForTargetFramework = namer.UniqueForTargetFramework;
        UniqueForTargetFrameworkName = namer.UniqueForTargetFrameworkName;
        UniqueForAssemblyConfiguration = namer.UniqueForAssemblyConfiguration;
        UniqueForAssemblyConfigurationValue = namer.UniqueForAssemblyConfigurationValue;
        UniqueForRuntimeAndVersion = namer.UniqueForRuntimeAndVersion;
        UniqueForTargetFrameworkAndVersion = namer.UniqueForTargetFrameworkAndVersion;
        UniqueForArchitecture = namer.UniqueForArchitecture;
        UniqueForOSPlatform = namer.UniqueForOSPlatform;
    }

    internal static (string runtime, Version Version) GetRuntimeAndVersion()
    {
#if NET6_0
        return ("DotNet", new(6, 0));
#elif NET7_0
        return ("DotNet", new(7, 0));
#elif NET8_0
        return ("DotNet", new(8, 0));
#elif NETFRAMEWORK
        // Mono can only be detected at runtime, and will use .NET Framework targets, so we have to check it first.
        if (RuntimeInformation.FrameworkDescription.StartsWith("Mono", StringComparison.OrdinalIgnoreCase))
        {
            return ("Mono", Environment.Version.MajorMinor());
        }

        // It's one of the .NET Framework versions we're explicitly targeting.
#if NET472
        return ("Net", new(4, 7));
#elif NET48
        return ("Net", new(4, 8));
#endif

        // It's only possible to get here if we've started compiling Verify for a new .NET Framework target
        // and forgot to add it to the list above.  Thus "not implemented" is appropriate.
#pragma warning disable CS0162
        throw new NotImplementedException();
#pragma warning restore CS0162

#else
        var description = RuntimeInformation.FrameworkDescription;

        if (description.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
        {
            var version = Version.Parse(description.Remove(".NET Framework "));
            return ("Net", version.MajorMinor());
        }

        if (description.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase))
        {
            var version = Version.Parse(description.Remove(".NETFramework "));
            return ("Net", version.MajorMinor());
        }

        if (description.StartsWith(".NET", StringComparison.OrdinalIgnoreCase))
        {
            return ("DotNet", Environment.Version.MajorMinor());
        }

        if (description.StartsWith("Mono", StringComparison.OrdinalIgnoreCase))
        {
            return ("Mono", Environment.Version.MajorMinor());
        }

        throw new($"Could not resolve runtime for '{description}'.");
#endif
    }
}