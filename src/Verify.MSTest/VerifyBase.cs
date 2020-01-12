using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        static VerifyBase()
        {
            InnerVerifier.Init(
                message => new AssertFailedException(message),
                input => CounterContext.Current.IntOrNext(input),
                input => CounterContext.Current.IntOrNext(input),
                input => CounterContext.Current.IntOrNext(input));
        }

        public TestContext TestContext
        {
            get => testContext;
            set { testContext = value; }
        }

        TestContext testContext = null!;

        DisposableVerifier BuildVerifier(string sourceFile, VerifySettings? settings)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            var type = GetType();

            var methodInfo = type.GetMethod(testContext.TestName, BindingFlags.Instance | BindingFlags.Public);

            var uniqueTestName = TestNameBuilder.GetUniqueTestName(type, methodInfo, settings.GetParameters());
            return new DisposableVerifier(type, Path.GetDirectoryName(sourceFile), uniqueTestName);
        }
    }
}