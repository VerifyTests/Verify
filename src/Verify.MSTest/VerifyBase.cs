using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public TestContext TestContext { get; set; } = null!;

        InnerVerifier BuildVerifier(string sourceFile, VerifySettings? settings)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            var type = GetType();

            var methodInfo = type.GetMethod(TestContext.TestName, BindingFlags.Instance | BindingFlags.Public);

            var parameters = settings.GetParameters(methodInfo);
            var uniqueTestName = TestNameBuilder.GetUniqueTestName(type, methodInfo, parameters);
            return new InnerVerifier(uniqueTestName, sourceFile, type.Assembly);
        }
    }
}