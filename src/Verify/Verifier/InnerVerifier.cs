using System;
using System.Collections.Generic;
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
        VerifySettings settings;
        FileNameBuilder fileNameBuilder;

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method, IReadOnlyList<object?>? parameters)
        {
            var (projectDirectory, replacements) = AttributeReader.GetAssemblyInfo(type.Assembly);
            settings.instanceScrubbers.Add(replacements);
            fileNameBuilder = new FileNameBuilder(method, type, projectDirectory, sourceFile, parameters, settings);

            this.settings = settings;

            CounterContext.Start();
        }

        public void Dispose()
        {
            CounterContext.Stop();
        }

        Task VerifyString(string target)
        {
            return VerifyBinary(Enumerable.Empty<ConversionStream>(),target, null);
        }
    }
}