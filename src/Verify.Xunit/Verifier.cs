using System;
using System.IO;
using Xunit.Sdk;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile)
        {
            var context = Context.Get();

            var testCase = context.TestCase;
            var type = (ReflectionTypeInfo) testCase.TestMethod.TestClass.Class;
            var method = (ReflectionMethodInfo) testCase.TestMethod.Method;
            var parameters = testCase.TestMethodArguments ??
                             testCase.DataRow ??
                             Array.Empty<object>();
            var name = TestNameBuilder.GetUniqueTestName(type.Type, method.MethodInfo, parameters);
            return new DisposableVerifier(type.Type, Path.GetDirectoryName(sourceFile), name);
        }
    }
}