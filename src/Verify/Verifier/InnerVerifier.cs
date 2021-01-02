﻿using System;
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
        string directory;
        string testName;
        Assembly assembly;
        VerifySettings settings;

        public InnerVerifier(string sourceFile, Assembly assembly, VerifySettings settings, MethodInfo method, IReadOnlyList<object?>? parameters)
        {
            var (projectDirectory, replacements) = AttributeReader.GetAssemblyInfo(assembly);
            directory = VerifierSettings.DeriveDirectory(sourceFile, projectDirectory);
            testName = TestNameBuilder.GetUniqueTestName(method, parameters);
            this.assembly = assembly;
            this.settings = settings;

            settings.instanceScrubbers.Add(replacements);

            CounterContext.Start();
        }

        FilePair GetFileNames(string extension)
        {
            return FileNameBuilder.GetFileNames(extension, settings.Namer, directory, testName, assembly);
        }

        FilePair GetFileNames(string extension, string suffix)
        {
            return FileNameBuilder.GetFileNames(extension, suffix, settings.Namer, directory, testName, assembly);
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