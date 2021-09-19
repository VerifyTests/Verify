using VerifyTests;

static class PrefixUnique
{
    static ConcurrentDictionary<string, MethodInfo> prefixList = new();

    public static void CheckPrefixIsUnique(string prefix, MethodInfo method)
    {
        prefixList.AddOrUpdate(
            prefix,
            _ => method,
            (_, info) => throw new($@"The prefix has already been used. Existing: {info.FullName()}. New: {method.FullName()}.
This is mostly caused by a conflicting combination of `VerifierSettings.DerivePathInfo()`, `UseMethodName.UseDirectory()`, `UseMethodName.UseTypeName()`, and `UseMethodName.UseMethodName()`. Prefix: {prefix}"));
    }

    public static void Clear()
    {
        prefixList = new();
    }

    public static string GetUniquenessParts(Namer namer, Assembly assembly)
    {
        var builder = new StringBuilder();
        if (namer.UniqueForRuntimeAndVersion || VerifierSettings.SharedNamer.UniqueForRuntimeAndVersion)
        {
            builder.Append($".{Namer.RuntimeAndVersion}");
        }
        else if (namer.UniqueForRuntime || VerifierSettings.SharedNamer.UniqueForRuntime)
        {
            builder.Append($".{Namer.Runtime}");
        }

        if (namer.UniqueForAssemblyConfiguration || VerifierSettings.SharedNamer.UniqueForAssemblyConfiguration)
        {
            builder.Append($".{assembly.GetAttributeConfiguration()}");
        }

        if (namer.UniqueForArchitecture || VerifierSettings.SharedNamer.UniqueForArchitecture)
        {
            builder.Append($".{Namer.Architecture}");
        }

        if (namer.UniqueForOSPlatform || VerifierSettings.SharedNamer.UniqueForOSPlatform)
        {
            builder.Append($".{Namer.OperatingSystemPlatform}");
        }

        return builder.ToString();
    }
}