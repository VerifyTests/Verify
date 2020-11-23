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

            var methodInfo = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == TestContext.TestName);

            if (methodInfo == null)
            {
                throw new($"Could not find method `{type.Name}.{TestContext.TestName}`");
            }

            var parameters = settings.GetParameters(methodInfo);
            var uniqueTestName = TestNameBuilder.GetUniqueTestName(type, methodInfo, parameters);
            return new(uniqueTestName, sourceFile, type.Assembly, settings);
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