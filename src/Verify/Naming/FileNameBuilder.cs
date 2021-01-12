using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using VerifyTests;

class FileNameBuilder
{
    Namer namer;
    MethodInfo method;
    Type type;
    static ConcurrentDictionary<string, MethodInfo> prefixList = new();
    string directory { get; }
    string testPrefix;
    string filePathPrefix;

    public FileNameBuilder(Namer namer, MethodInfo method, Type type, string projectDirectory, string sourceFile, IReadOnlyList<object?>? parameters, VerifySettings settings)
    {
        this.namer = namer;
        this.method = method;
        this.type = type;

        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, projectDirectory, type, method);

        var directoryValue = settings.directory ?? pathInfo.Directory;

        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
        if (directoryValue == null)
        {
            directoryValue = sourceFileDirectory;
        }
        else
        {
            directoryValue = Path.Combine(sourceFileDirectory, directoryValue);
            Directory.CreateDirectory(directoryValue);
        }

        var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
        var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

        directory = directoryValue;
        if (parameters == null || !parameters.Any())
        {
            testPrefix = $"{typeName}.{methodName}";
        }
        else
        {
            testPrefix = $"{typeName}.{methodName}_{ParameterBuilder.Concat(method, parameters)}";
        }

        filePathPrefix = GetPrefix();
    }

    static string GetTypeName(Type type)
    {
        if (type.IsNested)
        {
            return $"{type.ReflectedType!.Name}.{type.Name}";
        }

        return type.Name;
    }

    public FilePair GetFileNames(string extension, string? suffix = null)
    {
        string fullPrefix;
        if (suffix == null)
        {
            fullPrefix = filePathPrefix;
        }
        else
        {
            fullPrefix = $"{filePathPrefix}.{suffix}";
        }

        return new(extension, fullPrefix);
    }

    string GetPrefix()
    {
        StringBuilder builder = new(Path.Combine(directory, testPrefix));
        AppendFileParts(builder);
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

    public IEnumerable<string> GetVerifiedFiles(string extension)
    {
        var pattern = GetPattern(extension, testPrefix, "verified");
        return Directory.EnumerateFiles(directory, pattern);
    }

    public IEnumerable<string> GetReceivedFiles(string extension)
    {
        var pattern = GetPattern(extension, testPrefix, "received");
        return Directory.EnumerateFiles(directory, pattern);
    }

    string GetPattern(string extension, string testPrefix, string type)
    {
        StringBuilder builder = new(testPrefix);
        AppendFileParts(builder);
        builder.Append($".*.{type}.{extension}");
        return builder.ToString();
    }

    void AppendFileParts(StringBuilder builder)
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
            builder.Append($".{type.Assembly.GetAttributeConfiguration()}");
        }
    }
}