using System;
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

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method)
        {
            var (projectDirectory, replacements) = AttributeReader.GetAssemblyInfo(type.Assembly);
            settings.instanceScrubbers.Add(replacements);
            fileNameBuilder = new(method, type, projectDirectory, sourceFile, settings);

            this.settings = settings;

            VerifierSettings.RunBeforeCallbacks();
        }

        public void Dispose()
        {
            VerifierSettings.RunAfterCallbacks();
        }
    }
}