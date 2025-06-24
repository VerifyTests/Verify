static class PrefixUnique
{
    static ConcurrentBag<string> prefixList = [];

    public static void CheckPrefixIsUnique(string prefix)
    {
        if (prefixList.Contains(prefix))
        {
            throw new(
                $"""
                 The prefix has already been used: {prefix}. This is mostly caused one of the following:

                  * A conflicting combination of the following APIs:
                    * The static `VerifierSettings.DerivePathInfo()`
                    * The fluent APIs `Verify(...).UseDirectory()`, `Verify(...).UseTypeName()`, or `Verify(...).UseMethodName()`
                    * The instance `VerifySettings` APIs `settings.UseDirectory()`, `settings.UseTypeName()`, or `settings.UseMethodName()`
                  * Multiple calls to Verify or Throws in the same test method

                 If that's not the case, and having multiple identical prefixes is acceptable, then call `VerifierSettings.DisableRequireUniquePrefix()` to disable this uniqueness validation.
                 """);
        }

        prefixList.Add(prefix);
    }

    public static void Clear() =>
        prefixList = [];

    public static UniquenessList SharedUniqueness(Namer namer)
    {
        var builder = new UniquenessList();

        AppendTargetFramework(namer, builder);

        AppendAssemblyConfiguration(namer, builder);

        AppendArchitecture(namer, builder);

        AppendOsPlatform(namer, builder);

        return builder;
    }

    static void AppendTargetFramework(Namer namer, UniquenessList uniqueness)
    {
        var name = namer.UniqueForTargetFrameworkName;
        if (namer.ResolveUniqueForTargetFrameworkAndVersion())
        {
            if (name is null)
            {
                uniqueness.Add(Namer.TargetFrameworkNameAndVersion);
                return;
            }

            uniqueness.Add(name.NameAndVersion);
            return;
        }

        if (namer.ResolveUniqueForTargetFramework())
        {
            if (name is null)
            {
                uniqueness.Add(Namer.TargetFrameworkName);
                return;
            }

            uniqueness.Add(name.Name);
        }
    }

    static void AppendAssemblyConfiguration(Namer namer, UniquenessList uniqueness)
    {
        if (!namer.ResolveUniqueForAssemblyConfiguration())
        {
            return;
        }

        var configuration = namer.ResolveUniqueForAssemblyConfigurationValue();

        if (configuration is null)
        {
            uniqueness.Add(Namer.AssemblyConfig);
            return;
        }

        uniqueness.Add(configuration);
    }

    static void AppendArchitecture(Namer namer, UniquenessList uniqueness)
    {
        if (namer.ResolveUniqueForArchitecture())
        {
            uniqueness.Add(Namer.Architecture);
        }
    }

    static void AppendOsPlatform(Namer namer, UniquenessList uniqueness)
    {
        if (namer.ResolveUniqueForOSPlatform())
        {
            uniqueness.Add(Namer.OperatingSystemPlatform);
        }
    }
}