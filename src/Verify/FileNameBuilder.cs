using System;
using System.IO;
using System.Text;
using Verify;

static class FileNameBuilder
{
    public static (string received, string verified) GetFileNames(string extension, string? suffix, Namer namer, Type testType, string directory, string testName)
    {
        var filePrefix = GetFilePrefix(namer, testType, directory, testName);
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

    static string GetFilePrefix(Namer namer, Type testType, string directory, string testName)
    {
        var builder = new StringBuilder(Path.Combine(directory, testName));

        if (namer.UniqueForRuntimeAndVersion || Global.Namer.UniqueForRuntimeAndVersion)
        {
            builder.Append($".{Namer.runtimeAndVersion}");
        }
        else if (namer.UniqueForRuntime || Global.Namer.UniqueForRuntime)
        {
            builder.Append($".{Namer.runtime}");
        }

        if (namer.UniqueForAssemblyConfiguration || Global.Namer.UniqueForAssemblyConfiguration)
        {
            builder.Append($".{testType.Assembly.GetAttributeConfiguration()}");
        }

        return builder.ToString();
    }
}