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

        public FileNameBuilder(
            MethodInfo method,
            Type type,
            string projectDirectory,
            string sourceFile,
            VerifySettings settings)
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

            var fileNamePrefix = GetFileNamePrefix(method, type, settings, pathInfo, namer);
            filePathPrefix = Path.Combine(directory, fileNamePrefix);
            CheckPrefixIsUnique(filePathPrefix, method);

            var pattern = $"{fileNamePrefix}.*.*";
            var files = Directory.EnumerateFiles(directory, pattern).ToList();
            VerifiedFiles = FindMatchingFiles(files, fileNamePrefix, ".verified").ToList();
            ReceivedFiles = FindMatchingFiles(files, fileNamePrefix, ".received").ToList();
        }

        static IEnumerable<string> FindMatchingFiles(List<string> files, string fileNamePrefix, string suffix)
        {
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!name.StartsWith(fileNamePrefix))
                {
                    continue;
                }

                if (!name.EndsWith(suffix))
                {
                    continue;
                }

                var nameLength = name.Length - fileNamePrefix.Length;
                var prefixRemoved = name
                    .Substring(fileNamePrefix.Length, nameLength);
                if (prefixRemoved == suffix)
                {
                    yield return file;
                    continue;
                }

                var numberPart = prefixRemoved
                    .Substring(1, prefixRemoved.Length - suffix.Length - 1);

                if (int.TryParse(numberPart, out _))
                {
                    yield return file;
                }
            }
        }

        static string GetFileNamePrefix(MethodInfo method, Type type, VerifySettings settings, PathInfo pathInfo, Namer namer)
        {
            var uniquenessParts = GetUniquenessParts(namer, type);
            if (settings.fileName != null)
            {
                return settings.fileName + uniquenessParts;
            }

            var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
            var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

            var parameterText = GetParameterText(method, settings);

            return $"{typeName}.{methodName}{parameterText}{uniquenessParts}";
        }

        static string GetParameterText(MethodInfo method, VerifySettings settings)
        {
            var methodParameters = method.GetParameters();
            if (methodParameters.Any())
            {
                if (settings.parametersText != null)
                {
                    return $"_{settings.parametersText}";
                }

                if (settings.parameters != null)
                {
                    return $"_{ParameterBuilder.Concat(method, settings.parameters)}";
                }

                var names = string.Join(", ", methodParameters.Select(x => x.Name));
                throw new($@"Method `{method.DeclaringType!.Name}.{method.Name}` requires parameters, but none have been defined. Add UseParameters. For example:

VerifySettings settings = new();
settings.UseParameters({names});
await Verifier.Verify(target, settings);

or

await Verifier.Verify(target).UseParameters({names});
");
            }

            return "";
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
            if (prefixList.TryGetValue(prefix, out var existing))
            {
                throw new($@"The prefix has already been used. Existing: {existing.FullName()}. New: {method.FullName()}.
This is mostly caused by a conflicting combination of `VerifierSettings.DerivePathInfo()`, `UseMethodName.UseDirectory()`, `UseMethodName.UseTypeName()`, and `UseMethodName.UseMethodName()`. Prefix: {prefix}");
            }

            prefixList[prefix] = method;
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

            if (namer.UniqueForArchitecture || VerifierSettings.SharedNamer.UniqueForArchitecture)
            {
                builder.Append($".{Namer.Architecture}");
            }

            if (namer.UniqueForOSPlatform || VerifierSettings.SharedNamer.UniqueForOSPlatform)
            {
                builder.Append($".{Namer.OperatingSystemPlatform}");
            }

            return builder.ToString();
        }
    }
}