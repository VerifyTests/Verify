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

    static void AppendTargetFramework(Namer namer, UniquenessList builder)
    {
        var name = namer.UniqueForTargetFrameworkName;
        if (namer.ResolveUniqueForTargetFrameworkAndVersion())
        {
            if (name is null)
            {
                builder.Add(Namer.TargetFrameworkNameAndVersion);
                return;
            }

            builder.Add(name.NameAndVersion);
            return;
        }

        if (namer.ResolveUniqueForTargetFramework())
        {
            if (name is null)
            {
                builder.Add(Namer.TargetFrameworkName);
                return;
            }

            builder.Add(name.Name);
        }
    }

    static void AppendAssemblyConfiguration(Namer namer, UniquenessList builder)
    {
        if (!namer.ResolveUniqueForAssemblyConfiguration())
        {
            return;
        }

        var configuration = namer.ResolveUniqueForAssemblyConfigurationValue();

        if (configuration is null)
        {
            builder.Add(Namer.AssemblyConfig);
            return;
        }

        builder.Add(configuration);
    }

    static void AppendArchitecture(Namer namer, UniquenessList builder)
    {
        if (namer.ResolveUniqueForArchitecture())
        {
            builder.Add(Namer.Architecture);
        }
    }

    static void AppendOsPlatform(Namer namer, UniquenessList builder)
    {
        if (namer.ResolveUniqueForOSPlatform())
        {
            builder.Add(Namer.OperatingSystemPlatform);
        }
    }
}