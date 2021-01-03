using System.IO;
using System.Reflection;
using System.Text;
using VerifyTests;

static class FileNameBuilder
{
    public static FilePair GetFileNames(string extension, Namer namer, string directory, string testName, Assembly assembly, string? suffix = null)
    {
        StringBuilder builder = new(Path.Combine(directory, testName));
        AppendFileParts(namer, builder, assembly);
        if (suffix != null)
        {
            builder.Append($".{suffix}");
        }
        return new(extension, builder.ToString());
    }

    public static string GetVerifiedPattern(string extension, Namer namer, string testName, Assembly assembly)
    {
        return GetPattern(extension, namer, testName, assembly, "verified");
    }

    public static string GetReceivedPattern(string extension, Namer namer, string testName, Assembly assembly)
    {
        return GetPattern(extension, namer, testName, assembly, "received");
    }

    static string GetPattern(string extension, Namer namer, string testName, Assembly assembly, string type)
    {
        StringBuilder builder = new(testName);
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