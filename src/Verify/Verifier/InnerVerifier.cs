using System;
using System.Collections.Generic;
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
        VerifySettings settings;
        FileNameBuilder fileNameBuilder;

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method, IReadOnlyList<object?>? parameters)
        {
            var (projectDirectory, replacements) = AttributeReader.GetAssemblyInfo(type.Assembly);
            settings.instanceScrubbers.Add(replacements);
            fileNameBuilder = new FileNameBuilder(settings.Namer, method, type, projectDirectory, sourceFile, parameters, settings);

            this.settings = settings;

            CounterContext.Start();
        }

        public void Dispose()
        {
            CounterContext.Stop();
        }

        async Task HandleResults(List<ResultBuilder> results, VerifyEngine engine)
        {
            async Task HandleBuilder(ResultBuilder item, string? suffix = null)
            {
                var file = fileNameBuilder.GetFileNames(item.Extension, suffix);
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