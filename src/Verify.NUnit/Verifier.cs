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
            InnerVerifier.Init(
                message => new NUnitException(message),
                input => CounterContext.Current.IntOrNext(input),
                input => CounterContext.Current.IntOrNext(input),
                input => CounterContext.Current.IntOrNext(input));
        }

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
            return new DisposableVerifier(type, Path.GetDirectoryName(sourceFile), name);
        }
    }
}