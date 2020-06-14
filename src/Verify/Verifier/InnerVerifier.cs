using System;
using System.Reflection;

namespace VerifyTesting
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
        internal static Func<string, Exception> exceptionBuilder = null!;

        public static void Init(Func<string, Exception> exceptionBuilder)
        {
            InnerVerifier.exceptionBuilder = exceptionBuilder;
        }

        public InnerVerifier(string testName, string sourceFile, Assembly assembly)
        {
            directory = VerifierSettings.DeriveDirectory(sourceFile, assembly);
            this.testName = testName;
            this.assembly = assembly;
            CounterContext.Start();
        }

        FilePair GetFileNames(string extension, Namer namer)
        {
            return FileNameBuilder.GetFileNames(extension, namer, directory, testName, assembly);
        }

        FilePair GetFileNames(string extension, Namer namer, string suffix)
        {
            return FileNameBuilder.GetFileNames(extension, suffix, namer, directory, testName, assembly);
        }

        public void Dispose()
        {
            CounterContext.Stop();
        }
    }
}