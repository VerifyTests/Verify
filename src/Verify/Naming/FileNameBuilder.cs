using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using VerifyTests;

class FileNameBuilder
{
    static ConcurrentDictionary<string, MethodInfo> prefixList = new();

    public string GetPrefix(Namer namer, string directory, string testPrefix, Assembly assembly, MethodInfo method)
    {
        StringBuilder builder = new(Path.Combine(directory, testPrefix));
        AppendFileParts(namer, builder, assembly);
        var prefix = builder.ToString();
        if (prefixList.TryAdd(prefix, method))
        {
            return prefix;
        }

        var existing = prefixList[prefix];

        throw new($"The prefix has already been used. Existing: {existing.FullName()}. New: {method.FullName()}. This is mostly caused by a conflicting combination of `VerifierSettings.DerivePathInfo()`, `UseMethodName.UseDirectory()`, `UseMethodName.UseTypeName()`, and `UseMethodName.UseMethodName()`. Prefix: {prefix}");
    }

    internal static void ClearPrefixList()
    {
        prefixList = new();
    }

    public string GetVerifiedPattern(string extension, Namer namer, string testPrefix, Assembly assembly)
    {
        return GetPattern(extension, namer, testPrefix, assembly, "verified");
    }

    public string GetReceivedPattern(string extension, Namer namer, string testPrefix, Assembly assembly)
    {
        return GetPattern(extension, namer, testPrefix, assembly, "received");
    }

    string GetPattern(string extension, Namer namer, string testPrefix, Assembly assembly, string type)
    {
        StringBuilder builder = new(testPrefix);
        AppendFileParts(namer, builder, assembly);
        builder.Append($".*.{type}.{extension}");
        return builder.ToString();
    }

    void AppendFileParts(Namer namer, StringBuilder builder, Assembly assembly)
    {
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
    }
}