using VerifyTests;

static class PrefixUnique
{
    static ConcurrentBag<string> prefixList = new();

    public static void CheckPrefixIsUnique(string prefix)
    {
        if (prefixList.Contains(prefix))
        {
            throw new($@"The prefix has already been used: {prefix}.
This is mostly caused by a conflicting combination of `VerifierSettings.DerivePathInfo()`, `UseMethodName.UseDirectory()`, `UseMethodName.UseTypeName()`, and `UseMethodName.UseMethodName()`.
If that's not the case, and having multiple identical prefixes is acceptable, then call `VerifierSettings.DisableRequireUniquePrefix()` to disable this uniqueness validation.");
        }

        prefixList.Add(prefix);
    }

    public static void Clear()
    {
        prefixList = new();
    }

    public static string GetUniqueness(Namer namer)
    {
        var builder = new StringBuilder();

        AppendRuntime(namer, builder);

        AppendTargetFramework(namer, builder);

        AppendAssemblyConfiguration(namer, builder);

        AppendArchitecture(namer, builder);

        AppendOsPlatform(namer, builder);

        return builder.ToString();
    }

    static void AppendTargetFramework(Namer namer, StringBuilder builder)
    {
        if (namer.UniqueForTargetFrameworkAndVersion ||
            VerifierSettings.SharedNamer.UniqueForTargetFrameworkAndVersion)
        {
            var assembly = namer.UniqueForTargetFrameworkAssembly ??
                           VerifierSettings.SharedNamer.UniqueForTargetFrameworkAssembly;

            if (assembly == null)
            {
                builder.Append($".{Namer.TargetFrameworkNameAndVersion}");
                return;
            }
            
            var name = assembly.FrameworkName();

            if (name == null)
            {
                throw new($"UniqueForTargetFrameworkAndVersion used but no `TargetFrameworkAttribute` found in {assembly.FullName}.");
            }

            builder.Append($".{Namer.GetSimpleFrameworkName(name)}{name.Version.Major}_{name.Version.Minor}");
            return;
        }

        if (namer.UniqueForTargetFramework ||
            VerifierSettings.SharedNamer.UniqueForTargetFramework)
        {
            var assembly = namer.UniqueForTargetFrameworkAssembly ??
                         VerifierSettings.SharedNamer.UniqueForTargetFrameworkAssembly;

            if (assembly == null)
            {
                builder.Append($".{Namer.TargetFrameworkName}");
                return;
            }
            
            var name = assembly.FrameworkName();

            if (name == null)
            {
                throw new($"UniqueForTargetFramework used but no `TargetFrameworkAttribute` found in {assembly.FullName}.");
            }

            builder.Append($".{Namer.GetSimpleFrameworkName(name)}");
        }
    }

    static void AppendRuntime(Namer namer, StringBuilder builder)
    {
        if (namer.UniqueForRuntimeAndVersion ||
            VerifierSettings.SharedNamer.UniqueForRuntimeAndVersion)
        {
            builder.Append($".{Namer.RuntimeAndVersion}");
            return;
        }

        if (namer.UniqueForRuntime ||
            VerifierSettings.SharedNamer.UniqueForRuntime)
        {
            builder.Append($".{Namer.Runtime}");
        }
    }

    static void AppendAssemblyConfiguration(Namer namer, StringBuilder builder)
    {
        if (!namer.UniqueForAssemblyConfiguration &&
            !VerifierSettings.SharedNamer.UniqueForAssemblyConfiguration)
        {
            return;
        }

        var assembly = namer.UniqueForAssemblyConfigurationAssembly ??
                       VerifierSettings.SharedNamer.UniqueForAssemblyConfigurationAssembly;

        if (assembly == null)
        {
            builder.Append($".{Namer.AssemblyConfig}");
            return;
        }

        var config = assembly.Configuration();

        if (config == null)
        {
            throw new($"UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found in {assembly.FullName}.");
        }

        builder.Append($".{config}");
    }

    static void AppendArchitecture(Namer namer, StringBuilder builder)
    {
        if (!namer.UniqueForArchitecture &&
            !VerifierSettings.SharedNamer.UniqueForArchitecture)
        {
            return;
        }

        builder.Append($".{Namer.Architecture}");
    }

    static void AppendOsPlatform(Namer namer, StringBuilder builder)
    {
        if (!namer.UniqueForOSPlatform && !VerifierSettings.SharedNamer.UniqueForOSPlatform)
        {
            return;
        }

        builder.Append($".{Namer.OperatingSystemPlatform}");
    }
}