﻿using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        static FieldInfo field = typeof(TestContext.TestAdapter)
            .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic);

        static DisposableVerifier BuildVerifier(string sourceFile)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            var context = TestContext.CurrentContext;
            var adapter = context.Test;
            var test = (Test) field.GetValue(adapter);

            var type = test.TypeInfo.Type;
            var method = test.Method.MethodInfo;
            var name = TestNameBuilder.GetUniqueTestName(type, method, adapter.Arguments);
            return new DisposableVerifier(type, name, sourceFile);
        }
    }
}