using System.IO;
using System.Reflection;
using System.Text;
using VerifyTests;

static class FileNameBuilder
{
    public static FilePair GetFileNames(string extension, Namer namer, string directory, string testName, Assembly assembly)
    {
        var filePrefix = GetFilePrefix(namer, directory, testName, assembly);
        return new(extension, filePrefix);
    }

    public static FilePair GetFileNames(string extension, string suffix, Namer namer, string directory, string testName, Assembly assembly)
    {
        var filePrefix = GetFilePrefix(namer, directory, testName, assembly);
        return new(extension, $"{filePrefix}.{suffix}");
    }

    static string GetFilePrefix(Namer namer, string directory, string testName, Assembly assembly)
    {
        StringBuilder builder = new(Path.Combine(directory, testName));
        return AppendFileParts(namer, builder, assembly);
    }

    public static string GetVerifiedPattern(string extension, Namer namer, string testName, Assembly assembly)
    {
        StringBuilder builder = new(testName);
        var filePrefix = AppendFileParts(namer, builder, assembly);
        return $"{filePrefix}.*.verified.{extension}";
    }

    public static string GetReceivedPattern(string extension, Namer namer, string testName, Assembly assembly)
    {
        StringBuilder builder = new(testName);
        var filePrefix = AppendFileParts(namer, builder, assembly);
        return $"{filePrefix}.*received.{extension}";
    }

    static string AppendFileParts(Namer namer, StringBuilder builder, Assembly assembly)
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

        return builder.ToString();
    }
}