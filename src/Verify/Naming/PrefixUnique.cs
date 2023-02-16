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

    public static void Clear() =>
        prefixList = new();

    public static string SharedUniqueness(Namer namer)
    {
        var builder = new StringBuilder();

        AppendTargetFramework(namer, builder);

        AppendAssemblyConfiguration(namer, builder);

        AppendArchitecture(namer, builder);

        AppendOsPlatform(namer, builder);
        return builder.ToString();
    }

    static void AppendTargetFramework(Namer namer, StringBuilder builder)
    {
        if (namer.ResolveUniqueForTargetFrameworkAndVersion())
        {
            var assembly = namer.ResolveUniqueForTargetFrameworkAssembly();

            if (assembly is null)
            {
                builder.Append($".{Namer.TargetFrameworkNameAndVersion}");
                return;
            }

            var name = assembly.FrameworkName();

            if (name is null)
            {
                throw new($"UniqueForTargetFrameworkAndVersion used but no `TargetFrameworkAttribute` found in {assembly.FullName}.");
            }

            builder.Append($".{Namer.GetSimpleFrameworkName(name)}{name.Version.Major}_{name.Version.Minor}");
            return;
        }

        if (namer.ResolveUniqueForTargetFramework())
        {
            var assembly = namer.ResolveUniqueForTargetFrameworkAssembly();

            if (assembly is null)
            {
                builder.Append($".{Namer.TargetFrameworkName}");
                return;
            }

            var name = assembly.FrameworkName();

            if (name is null)
            {
                throw new($"UniqueForTargetFramework used but no `TargetFrameworkAttribute` found in {assembly.FullName}.");
            }

            builder.Append($".{Namer.GetSimpleFrameworkName(name)}");
        }
    }

    static void AppendAssemblyConfiguration(Namer namer, StringBuilder builder)
    {
        if (!namer.ResolveUniqueForAssemblyConfiguration())
        {
            return;
        }

        var configuration = namer.ResolveUniqueForAssemblyConfigurationValue();

        if (configuration is null)
        {
            builder.Append($".{Namer.AssemblyConfig}");
            return;
        }

        builder.Append($".{configuration}");
    }

    static void AppendArchitecture(Namer namer, StringBuilder builder)
    {
        if (namer.ResolveUniqueForArchitecture())
        {
            builder.Append($".{Namer.Architecture}");
        }
    }

    static void AppendOsPlatform(Namer namer, StringBuilder builder)
    {
        if (namer.ResolveUniqueForOSPlatform())
        {
            builder.Append($".{Namer.OperatingSystemPlatform}");
        }
    }
}