using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        static Verifier()
        {
            global::Verifier.Init(
                message => new NUnitException(message),
                input => CounterContext.Current.IntOrNext(input),
                input => CounterContext.Current.IntOrNext(input),
                input => CounterContext.Current.IntOrNext(input),
                Assert.AreEqual);
        }

        static FieldInfo field = typeof(TestContext.TestAdapter).GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic);

        static DisposableVerifier BuildVerifier(string sourceFile)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            var context = TestContext.CurrentContext;
            var testAdapter = context.Test;
            var test = (Test) field.GetValue(testAdapter);

            var testType = test.TypeInfo.Type;
            var uniqueTestName = TestNameBuilder.GetUniqueTestName(testType, test.Method.MethodInfo, testAdapter.Arguments);
            return new DisposableVerifier(testType, Path.GetDirectoryName(sourceFile), uniqueTestName);
        }
    }
}