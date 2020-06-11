using System;
using System.IO;
using System.Text;
using Verify;

static class FileNameBuilder
{
    public static FilePair GetFileNames(string extension, Namer namer, string directory, string testName)
    {
        var filePrefix = GetFilePrefix(namer, directory, testName);
        return new FilePair(extension, filePrefix);
    }

    public static FilePair GetFileNames(string extension, string suffix, Namer namer, string directory, string testName)
    {
        var filePrefix = GetFilePrefix(namer, directory, testName);
        return new FilePair(extension, $"{filePrefix}.{suffix}");
    }

    static string GetFilePrefix(Namer namer, string directory, string testName)
    {
        var builder = new StringBuilder(Path.Combine(directory, testName));
        return AppendFileParts(namer, builder);
    }

    public static string GetVerifiedPattern(string extension, Namer namer, string testName)
    {
        var builder = new StringBuilder(testName);
        var filePrefix = AppendFileParts(namer, builder);
        return $"{filePrefix}.*.verified.{extension}";
    }

    public static string GetReceivedPattern(string extension, Namer namer, string testName)
    {
        var builder = new StringBuilder(testName);
        var filePrefix = AppendFileParts(namer, builder);
        return $"{filePrefix}.*received.{extension}";
    }

    static string AppendFileParts(Namer namer, StringBuilder builder)
    {
        if (namer.UniqueForRuntimeAndVersion || SharedVerifySettings.SharedNamer.UniqueForRuntimeAndVersion)
        {
            builder.Append($".{Namer.RuntimeAndVersion}");
        }
        else if (namer.UniqueForRuntime || SharedVerifySettings.SharedNamer.UniqueForRuntime)
        {
            builder.Append($".{Namer.Runtime}");
        }

        if (namer.UniqueForAssemblyConfiguration || SharedVerifySettings.SharedNamer.UniqueForAssemblyConfiguration)
        {
            if (SharedVerifySettings.assembly == null)
            {
                throw new Exception("`UniqueForAssemblyConfiguration` requires `SharedVerifySettings.SetTestAssembly(Assembly.GetExecutingAssembly());` to be called at assembly startup.");
            }
            builder.Append($".{SharedVerifySettings.assembly.GetAttributeConfiguration()}");
        }

        return builder.ToString();
    }
}