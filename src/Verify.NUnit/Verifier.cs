using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Verify;

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
            if (SharedVerifySettings.assembly == null)
            {
                SharedVerifySettings.SetTestAssembly(test.TypeInfo.Assembly);
            }

            var method = test.Method.MethodInfo;
            var name = TestNameBuilder.GetUniqueTestName(Path.GetFileNameWithoutExtension(sourceFile), method, adapter.Arguments);
            return new DisposableVerifier(name, sourceFile);
        }
    }
}