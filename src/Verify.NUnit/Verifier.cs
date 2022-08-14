namespace VerifyNUnit;

public static partial class Verifier
{
    static FieldInfo field;

    static Verifier()
    {
        var temp = typeof(TestContext.TestAdapter)
            .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic);

        field = temp ?? throw new("Could not find field `_test` on TestContext.TestAdapter.");
    }

    static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        var context = TestContext.CurrentContext;
        var adapter = context.Test;
        var test = (Test) field.GetValue(adapter)!;
        if (test.TypeInfo == null || test.Method is null)
        {
            throw new("Expected Test.TypeInfo and Test.Method to not be null. Raise a Pull Request with a test that replicates this problem.");
        }

        if (settings.parameters == null &&
            adapter.Arguments.Any())
        {
            settings.parameters = adapter.Arguments;
        }

        var fullName = test.FullName;
        var isCustomFullName = !fullName.StartsWith($"{test.TypeInfo.FullName}.{test.Method.Name}");
        if (isCustomFullName)
        {
            if (settings.typeName == null)
            {
                var fullNameLength = fullName.Length - (test.Name.Length + 1);
                settings.typeName = fullName[..fullNameLength].Replace("\"", "").ReplaceInvalidPathChars();
            }

            if (settings.methodName == null)
            {
                settings.methodName = test.Name.ReplaceInvalidPathChars();
            }
        }

        var type = test.TypeInfo!.Type;
        TargetAssembly.Assign(type.Assembly);

        var method = test.Method!.MethodInfo;
        GetFileConvention fileConvention = (uniquenessForReceived, uniquenessForVerified) =>
            ReflectionFileNameBuilder.FileNamePrefix(method, type, sourceFile, settings, uniquenessForReceived, uniquenessForVerified);

        return new(sourceFile, settings, fileConvention);
    }

    static SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task<VerifyResult>> verify)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = BuildVerifier(sourceFile, verifySettings);
                return await verify(verifier);
            });
    }
}