using System;
using System.IO;
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

        static DisposableVerifier BuildVerifier(string sourceFile)
        {
            var context = TestContext.CurrentContext;
            var test = context.Test;
            var type = Type.GetType(test.ClassName);
            return new DisposableVerifier(type, Path.GetDirectoryName(sourceFile), test.Name);
        }
    }
}