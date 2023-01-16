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
            targetFrameworkName = GetSimpleFrameworkName(frameworkName);
            targetFrameworkNameAndVersion = $"{targetFrameworkName}{frameworkName.Version.Major}_{frameworkName.Version.Minor}";
        }
    }

    public static string GetSimpleFrameworkName(FrameworkName name)
    {
        var identifier = name.Identifier;

        if (identifier.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
        {
            return "Net";
        }

        if (string.Equals(identifier, ".NETCoreApp", StringComparison.OrdinalIgnoreCase))
        {
            if (name.Version.Major < 5)
            {
                return "Core";
            }
        }

        if (identifier.StartsWith("NETCore", StringComparison.OrdinalIgnoreCase))
        {
            return "Core";
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

    internal Assembly? UniqueForTargetFrameworkAssembly;

    internal Assembly? ResolveUniqueForTargetFrameworkAssembly() =>
        UniqueForTargetFrameworkAssembly ??
        VerifierSettings.SharedNamer.UniqueForTargetFrameworkAssembly;

    internal bool UniqueForAssemblyConfiguration;

    internal bool ResolveUniqueForAssemblyConfiguration() =>
        UniqueForAssemblyConfiguration ||
        VerifierSettings.SharedNamer.UniqueForAssemblyConfiguration;

    internal Assembly? UniqueForAssemblyConfigurationAssembly;

    internal Assembly? ResolveUniqueForAssemblyConfigurationAssembly() =>
        UniqueForAssemblyConfigurationAssembly ??
        VerifierSettings.SharedNamer.UniqueForAssemblyConfigurationAssembly;

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
        Architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLower();
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
        UniqueForTargetFrameworkAssembly = namer.UniqueForTargetFrameworkAssembly;
        UniqueForAssemblyConfiguration = namer.UniqueForAssemblyConfiguration;
        UniqueForAssemblyConfigurationAssembly = namer.UniqueForAssemblyConfigurationAssembly;
        UniqueForRuntimeAndVersion = namer.UniqueForRuntimeAndVersion;
        UniqueForTargetFrameworkAndVersion = namer.UniqueForTargetFrameworkAndVersion;
        UniqueForArchitecture = namer.UniqueForArchitecture;
        UniqueForOSPlatform = namer.UniqueForOSPlatform;
    }

    static (string runtime, Version Version) GetRuntimeAndVersion()
    {
#if NETCOREAPP2_1
        return ("Core", new(2, 1));
#elif NETCOREAPP2_2
        return ("Core", new(2, 2));
#elif NETCOREAPP3_0
        return ("Core", new(3, 0));
#elif NETCOREAPP3_1
        return ("Core", new(3, 1));
#elif NET462
        return ("Net", new(4, 6));
#elif NET472
        return ("Net", new(4, 7));
#else
        var description = RuntimeInformation.FrameworkDescription;

        if (description.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
        {
            var version = Version.Parse(description.Remove(".NET Framework "));
            return ("Net", version);
        }

        if (description.StartsWith(".NET", StringComparison.OrdinalIgnoreCase))
        {
            return ("DotNet", Environment.Version);
        }

        if (description.StartsWith("Mono", StringComparison.OrdinalIgnoreCase))
        {
            return ("Mono", Environment.Version);
        }

        throw new($"Could not resolve runtime for '{description}'.");
#endif
    }
}