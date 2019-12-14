using System;
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
            var context = TestContext.CurrentContext;
            var testAdapter = context.Test;
            var test = (Test)field.GetValue(testAdapter);
            return new DisposableVerifier(test.Method.MethodInfo.DeclaringType, Path.GetDirectoryName(sourceFile), testAdapter.FullName);
        }
    }
}