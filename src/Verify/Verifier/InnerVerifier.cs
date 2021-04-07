using System;
using System.Collections.Generic;
using System.Reflection;

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
            fileNameBuilder = new(method, type, projectDirectory, sourceFile, parameters, settings);

            this.settings = settings;

            CounterContext.Start();
        }

        public void Dispose()
        {
            CounterContext.Stop();
        }
    }
}