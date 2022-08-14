using NUnit.Framework.Interfaces;

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
        var typeInfo = test.TypeInfo;
        if (typeInfo == null || test.Method is null)
        {
            throw new("Expected Test.TypeInfo and Test.Method to not be null. Raise a Pull Request with a test that replicates this problem.");
        }

        if (settings.parameters == null &&
            adapter.Arguments.Any())
        {
            settings.parameters = adapter.Arguments;
        }

        if (test.IsCustomName())
        {
            settings.typeName ??= test.GetTypeName();

            settings.methodName ??= test.Name.ReplaceInvalidPathChars();
        }

        var type = typeInfo.Type;
        TargetAssembly.Assign(type.Assembly);

        var method = test.Method.MethodInfo;
        GetFileConvention fileConvention = (uniquenessForReceived, uniquenessForVerified) =>
            ReflectionFileNameBuilder.FileNamePrefix(method, type, sourceFile, settings, uniquenessForReceived, uniquenessForVerified);

        return new(sourceFile, settings, fileConvention);
    }

    static string GetTypeName(this ITest test)
    {
        var fullName = test.FullName;
        var fullNameLength = fullName.Length - (test.Name.Length + 1);
        var typeName = fullName[..fullNameLength];
        var typeInfo = test.TypeInfo!;
        if (typeInfo.Namespace != null)
        {
            typeName = typeName[(typeInfo.Namespace.Length + 1)..];
        }

        return typeName
            .Remove("\"")
            .ReplaceInvalidPathChars();
    }

    static bool IsCustomName(this ITest test) =>
        !test.FullName.StartsWith($"{test.TypeInfo!.FullName}.{test.Method!.Name}");

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