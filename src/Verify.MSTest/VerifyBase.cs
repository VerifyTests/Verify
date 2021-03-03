using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;

namespace VerifyMSTest
{
    [TestClass]
    public abstract partial class VerifyBase
    {
        public TestContext TestContext { get; set; } = null!;

        InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile)
        {
            var type = GetType();

            var methods = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var testName = TestContext.TestName;
            testName = testName.Substring(0, testName.IndexOf('('));
            var methodInfo = methods
                .FirstOrDefault(x => x.Name == testName);

            if (methodInfo == null)
            {
                throw new($"Could not find method `{type.Name}.{testName}`");
            }

            var parameters = settings.GetParameters(methodInfo);
            return new(sourceFile, type, settings, methodInfo, parameters);
        }

        SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task> verify)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            return new(
                settings,
                async verifySettings =>
                {
                    using var verifier = BuildVerifier(verifySettings, sourceFile);
                    await verify(verifier);
                });
        }
    }
}