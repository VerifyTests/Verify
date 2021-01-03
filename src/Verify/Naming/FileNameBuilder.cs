using System.IO;
using System.Reflection;
using System.Text;
using VerifyTests;

static class FileNameBuilder
{
    public static string GetPrefix(Namer namer, string directory, string testPrefix, Assembly assembly)
    {
        StringBuilder builder = new(Path.Combine(directory, testPrefix));
        AppendFileParts(namer, builder, assembly);
        return builder.ToString();
    }

    public static string GetVerifiedPattern(string extension, Namer namer, string testPrefix, Assembly assembly)
    {
        return GetPattern(extension, namer, testPrefix, assembly, "verified");
    }

    public static string GetReceivedPattern(string extension, Namer namer, string testPrefix, Assembly assembly)
    {
        return GetPattern(extension, namer, testPrefix, assembly, "received");
    }

    static string GetPattern(string extension, Namer namer, string testPrefix, Assembly assembly, string type)
    {
        StringBuilder builder = new(testPrefix);
        AppendFileParts(namer, builder, assembly);
        builder.Append($".*.{type}.{extension}");
        return builder.ToString();
    }

    static void AppendFileParts(Namer namer, StringBuilder builder, Assembly assembly)
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