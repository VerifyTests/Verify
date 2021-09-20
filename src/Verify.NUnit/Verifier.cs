using System.Linq;
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
            if (temp is null)
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
            if (test.TypeInfo == null || test.Method is null)
            {
                throw new("Expected Test.TypeInfo and Test.Method to not be null. Raise a Pull Request with a test that replicates this problem.");
            }

            if (adapter.Arguments.Any())
            {
                settings.parameters = adapter.Arguments;
            }
            var type = test.TypeInfo!.Type;
            Namer.UseAssemblyForConfig(type.Assembly);
            return new(
                sourceFile,
                type,
                settings,
                test.Method!.MethodInfo);
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