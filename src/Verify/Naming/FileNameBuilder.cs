using System;
using System.IO;
using System.Text;
using Verify;

static class FileNameBuilder
{
    public static (string received, string verified) GetFileNames(string extension, string? suffix, Namer namer, Type testType, string directory, string testName)
    {
        var builder = new StringBuilder(Path.Combine(directory, testName));
        var filePrefix = AppendFileParts(namer, testType, builder);
        if (suffix == null)
        {
            var received = $"{filePrefix}.received.{extension}";
            var verified = $"{filePrefix}.verified.{extension}";
            return (received, verified);
        }
        else
        {
            var received = $"{filePrefix}.{suffix}.received.{extension}";
            var verified = $"{filePrefix}.{suffix}.verified.{extension}";
            return (received, verified);
        }
    }

    public static string GetVerifiedPattern(string extension, Namer namer, Type testType, string testName)
    {
        var builder = new StringBuilder(testName);
        var filePrefix = AppendFileParts(namer, testType, builder);
        return $"{filePrefix}.*.verified.{extension}";
    }

    static string AppendFileParts(Namer namer, Type testType, StringBuilder builder)
    {
        if (namer.UniqueForRuntimeAndVersion || SharedVerifySettings.SharedNamer.UniqueForRuntimeAndVersion)
        {
            builder.Append($".{Namer.runtimeAndVersion}");
        }
        else if (namer.UniqueForRuntime || SharedVerifySettings.SharedNamer.UniqueForRuntime)
        {
            builder.Append($".{Namer.runtime}");
        }

        if (namer.UniqueForAssemblyConfiguration || SharedVerifySettings.SharedNamer.UniqueForAssemblyConfiguration)
        {
            builder.Append($".{testType.Assembly.GetAttributeConfiguration()}");
        }

        return builder.ToString();
    }
}