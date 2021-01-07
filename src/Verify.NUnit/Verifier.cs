using System;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VerifyTests;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        static FieldInfo field;

        static Verifier()
        {
            var temp = typeof(TestContext.TestAdapter)
                .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic);
            if (temp == null)
            {
                throw new("Could not find field `_test` on TestContext.TestAdapter.");
            }

            field = temp;
        }

        static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            var context = TestContext.CurrentContext;
            var adapter = context.Test;
            var test = (Test) field.GetValue(adapter)!;
            if (test.TypeInfo == null || test.Method == null)
            {
                throw new Exception("Expected Test.TypeInfo and Test.Method to not be null. Raise a Pull Request with a test that replicates this problem.");
            }

            return new(
                sourceFile,
                test.TypeInfo!.Type,
                settings,
                test.Method!.MethodInfo,
                adapter.Arguments);
        }

        static SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task> verify)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            return new(
                settings,
                async verifySettings =>
                {
                    using var verifier = BuildVerifier(sourceFile, verifySettings);
                    await verify(verifier);
                });
        }
    }
}