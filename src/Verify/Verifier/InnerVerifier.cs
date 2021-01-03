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

            filePathPrefix = FileNameBuilder.GetPrefix(settings.Namer, directory, testPrefix, assembly);
            this.settings = settings;

            settings.instanceScrubbers.Add(replacements);

            CounterContext.Start();
        }

        static (string directory, string methodName, string typeName) GetPathInfo(string sourceFile, Type type, VerifySettings settings, MethodInfo method, string projectDirectory)
        {
            var pathInfo = VerifierSettings.GetPathInfo(sourceFile, projectDirectory, type, method);
            var directory = pathInfo.Directory;

            if (settings.directory != null)
            {
                directory = settings.directory;
            }

            var sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
            directory = Path.Combine(sourceFileDirectory, directory);
            Directory.CreateDirectory(directory);

            var typeName = settings.typeName ?? pathInfo.TypeName;
            var methodName = settings.methodName ?? pathInfo.MethodName;

            return (directory, methodName, typeName);
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
            async Task HandleBuilder(ResultBuilder item, FilePair file)
            {
                var result = await item.GetResult(file);
                engine.HandleCompareResult(result, file);
            }

            if (results.Count == 1)
            {
                var item = results[0];
                var file = GetFileNames(item.Extension);
                await HandleBuilder(item, file);
                return;
            }

            for (var index = 0; index < results.Count; index++)
            {
                var item = results[index];
                var file = GetFileNames(item.Extension, $"{index:D2}");
                await HandleBuilder(item, file);
            }
        }
    }
}