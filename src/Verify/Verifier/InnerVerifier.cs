using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VerifyTests
{
    /// <summary>
    /// Not for public use.
    /// </summary>
    public partial class InnerVerifier :
        IDisposable
    {
        string directory;
        string testPrefix;
        Assembly assembly;
        VerifySettings settings;
        string filePathPrefix;

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method, IReadOnlyList<object?>? parameters)
        {
            assembly = type.Assembly;
            var (projectDirectory, replacements) = AttributeReader.GetAssemblyInfo(assembly);
            settings.instanceScrubbers.Add(replacements);

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

            filePathPrefix = FileNameBuilder.GetPrefix(settings.Namer, directory, testPrefix, assembly, method);
            this.settings = settings;

            CounterContext.Start();
        }

        static (string directory, string methodName, string typeName) GetPathInfo(string sourceFile, Type type, VerifySettings settings, MethodInfo method, string projectDirectory)
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

        FilePair GetFileNames(string extension, string? suffix = null)
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

        public void Dispose()
        {
            CounterContext.Stop();
        }

        async Task HandleResults(List<ResultBuilder> results, VerifyEngine engine)
        {
            async Task HandleBuilder(ResultBuilder item, string? suffix = null)
            {
                var file = GetFileNames(item.Extension, suffix);
                var result = await item.GetResult(file);

                engine.HandleCompareResult(result, file);
            }

            if (results.Count == 1)
            {
                var item = results[0];
                await HandleBuilder(item);
                return;
            }

            for (var index = 0; index < results.Count; index++)
            {
                var item = results[index];
                await HandleBuilder(item, $"{index:D2}");
            }
        }
    }
}