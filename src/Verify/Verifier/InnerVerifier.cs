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
        static List<(Action start, Action stop)> testCallbacks = new();
        VerifySettings settings;
        FileNameBuilder fileNameBuilder;

        public InnerVerifier(string sourceFile, Type type, VerifySettings settings, MethodInfo method, IReadOnlyList<object?>? parameters)
        {
            var (projectDirectory, replacements) = AttributeReader.GetAssemblyInfo(type.Assembly);
            settings.instanceScrubbers.Add(replacements);
            fileNameBuilder = new(method, type, projectDirectory, sourceFile, parameters, settings);

            this.settings = settings;

            foreach (var (start, _) in testCallbacks)
            {
                start();
            }
        }

        public static void AddTestCallback(Action start, Action stop)
        {
            testCallbacks.Add((start, stop));
        }

        public void Dispose()
        {
            foreach (var (_, stop) in testCallbacks)
            {
                stop();
            }
        }
    }
}