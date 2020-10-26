using System;
using System.Reflection;
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

            var methodInfo = type.GetMethod(TestContext.TestName, BindingFlags.Instance | BindingFlags.Public);

            if (methodInfo == null)
            {
                throw new Exception($"Could not find method `{type.Name}.{TestContext.TestName}`");
            }

            var parameters = settings.GetParameters(methodInfo);
            var uniqueTestName = TestNameBuilder.GetUniqueTestName(type, methodInfo, parameters);
            return new InnerVerifier(uniqueTestName, sourceFile, type.Assembly);
        }
    }
}