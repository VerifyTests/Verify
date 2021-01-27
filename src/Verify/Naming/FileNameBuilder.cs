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
        Namer namer;
        MethodInfo method;
        Type type;
        static ConcurrentDictionary<string, MethodInfo> prefixList = new();
        string directory;
        string testPrefix;
        string filePathPrefix;
        string fileParts;

        public FileNameBuilder(MethodInfo method, Type type, string projectDirectory, string sourceFile, IReadOnlyList<object?>? parameters, VerifySettings settings)
        {
            namer = settings.Namer;
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

            fileParts = GetFileParts();
            filePathPrefix = GetPrefix();

            var pattern = $"{testPrefix}{fileParts}.*.*";
            var files = Directory.EnumerateFiles(directory, pattern).ToList();
            VerifiedFiles = files.Where(x => x.Contains(".verified.")).ToList();
            ReceivedFiles = files.Where(x => x.Contains(".received.")).ToList();
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

        string GetPrefix()
        {
            StringBuilder builder = new(Path.Combine(directory, testPrefix));
            builder.Append(fileParts);
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

        string GetFileParts()
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