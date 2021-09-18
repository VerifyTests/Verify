using System.Linq;
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
                testName = testName[..indexOf];
            }

            indexOf = testName.IndexOf('.');
            if (indexOf > 0)
            {
                testName = testName[(indexOf + 1)..];
            }

            var methodInfo = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == testName);

            if (methodInfo is null)
            {
                throw new($"Could not find method `{type.Name}.{testName}`.");
            }

            return new(sourceFile, type, settings, methodInfo);
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