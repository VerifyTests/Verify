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
    string directory;
    string testPrefix;
    string filePathPrefix;

    public FileNameBuilder(Namer namer, MethodInfo method, Type type, string projectDirectory, string sourceFile, IReadOnlyList<object?>? parameters, VerifySettings settings)
    {
        this.namer = namer;
        this.method = method;
        this.type = type;
        var (directory, methodName, typeName) = GetPathInfo(sourceFile, type, settings, method, projectDirectory);

        this.directory = directory;
        if (parameters == null || !parameters.Any())
        {
            testPrefix = $"{typeName}.{methodName}";
        }
        else
        {
            testPrefix = $"{typeName}.{methodName}_{ParameterBuilder.Concat(method, parameters)}";
        }

        filePathPrefix = GetPrefix(directory, testPrefix);
    }

    (string directory, string methodName, string typeName) GetPathInfo(string sourceFile, Type type, VerifySettings settings, MethodInfo method, string projectDirectory)
    {
        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, projectDirectory, type, method);

        var directory = settings.directory ?? pathInfo.Directory;

        var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
        if (directory == null)
        {
            directory = sourceFileDirectory;
        }
        else
        {
            directory = Path.Combine(sourceFileDirectory, directory);
            Directory.CreateDirectory(directory);
        }

        var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
        var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

        return (directory, methodName, typeName);
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
    public string GetPrefix(string directory, string testPrefix)
    {
        StringBuilder builder = new(Path.Combine(directory, testPrefix));
        AppendFileParts(builder, type.Assembly);
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

    public string GetVerifiedPattern(string extension, Assembly assembly)
    {
        return GetPattern(extension, testPrefix, assembly, "verified");
    }

    public string GetReceivedPattern(string extension, Assembly assembly)
    {
        return GetPattern(extension, testPrefix, assembly, "received");
    }

    string GetPattern(string extension, string testPrefix, Assembly assembly, string type)
    {
        StringBuilder builder = new(testPrefix);
        AppendFileParts(builder, assembly);
        builder.Append($".*.{type}.{extension}");
        return builder.ToString();
    }

    void AppendFileParts(StringBuilder builder, Assembly assembly)
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