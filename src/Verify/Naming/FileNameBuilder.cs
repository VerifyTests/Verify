using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public class FileNameBuilder
    {
        static ConcurrentDictionary<string, MethodInfo> prefixList = new();
        string filePathPrefix;

        public FileNameBuilder(MethodInfo method, Type type, string projectDirectory, string sourceFile, IReadOnlyList<object?>? parameters, VerifySettings settings)
        {
            var namer = settings.Namer;

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

            var fileNamePrefix = GetFileNamePrefix(method, type, parameters, settings, pathInfo, namer);
            filePathPrefix = Path.Combine(directory, fileNamePrefix);
            CheckPrefixIsUnique(filePathPrefix, method);

            var pattern = $"{fileNamePrefix}.*.*";
            var files = Directory.EnumerateFiles(directory, pattern).ToList();
            VerifiedFiles = files.Where(x => x.Contains($"{fileNamePrefix}.verified.")).ToList();
            ReceivedFiles = files.Where(x => x.Contains($"{fileNamePrefix}.received.")).ToList();
        }

        static string GetFileNamePrefix(MethodInfo method, Type type, IReadOnlyList<object?>? parameters, VerifySettings settings, PathInfo pathInfo, Namer namer)
        {
            var uniquenessParts = GetUniquenessParts(namer, type);
            if (settings.fileName != null)
            {
                return settings.fileName + uniquenessParts;
            }

            var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
            var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

            string testPrefix;
            if (parameters == null || !parameters.Any())
            {
                testPrefix = $"{typeName}.{methodName}";
            }
            else
            {
                testPrefix = $"{typeName}.{methodName}_{ParameterBuilder.Concat(method, parameters)}";
            }

            return $"{testPrefix}{uniquenessParts}";
        }

        public List<string> VerifiedFiles { get; }

        public List<string> ReceivedFiles { get; }

        static string GetTypeName(Type type)
        {
            if (type.IsNested)
            {
                return $"{type.ReflectedType!.Name}.{type.Name}";
            }

            return type.Name;
        }

        public FilePair GetFileNames(string extension)
        {
            return new(extension, filePathPrefix);
        }

        public FilePair GetFileNames(string extension, int index)
        {
            return new(extension, $"{filePathPrefix}.{index:D2}");
        }

        static void CheckPrefixIsUnique(string prefix, MethodInfo method)
        {
            if (!prefixList.TryGetValue(prefix, out var existing))
            {
                prefixList[prefix] = method;
                return;
            }

            throw new($"The prefix has already been used. Existing: {existing.FullName()}. New: {method.FullName()}. This is mostly caused by a conflicting combination of `VerifierSettings.DerivePathInfo()`, `UseMethodName.UseDirectory()`, `UseMethodName.UseTypeName()`, and `UseMethodName.UseMethodName()`. Prefix: {prefix}");
        }

        public static void ClearPrefixList()
        {
            prefixList = new();
        }

        static string GetUniquenessParts(Namer namer, Type type)
        {
            StringBuilder builder = new();
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

            return builder.ToString();
        }
    }
}