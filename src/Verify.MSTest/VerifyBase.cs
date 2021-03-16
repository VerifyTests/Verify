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

            var testName = TestContext.TestName;
            var indexOf = testName.IndexOf('(');
            if (indexOf > 0)
            {
                testName = testName.Substring(0, indexOf);
            }

            indexOf = testName.IndexOf('.');
            if (indexOf > 0)
            {
                testName = testName.Substring(indexOf + 1);
            }

            var methodInfo = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == testName);

            if (methodInfo == null)
            {
                throw new($"Could not find method `{type.Name}.{testName}`.");
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