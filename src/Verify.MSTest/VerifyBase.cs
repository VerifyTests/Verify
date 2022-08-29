namespace VerifyMSTest;

[TestClass]
public abstract partial class VerifyBase
{
    public TestContext TestContext { get; set; } = null!;

    InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile)
    {
        var type = GetType();

        var testName = TestContext.TestName;
        var indexOf = testName.IndexOf('(');
        if (indexOf > 0)
        {
            testName = testName[..indexOf];
        }

        indexOf = testName.IndexOf('.');
        if (indexOf > 0)
        {
            testName = testName[(indexOf + 1)..];
        }

        TargetAssembly.Assign(type.Assembly);
        var method = type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(_ => _.Name == testName);

        if (method is null)
        {
            throw new($"Could not find method `{type.Name}.{testName}`.");
        }

        var methodParameters = method.ParameterNames();
        var typeName = type.NameWithParent();
        var methodName = method.Name;

        GetFileConvention fileConvention = (uniquenessReceived, uniquenessVerified) =>
            ReflectionFileNameBuilder.FileNamePrefix(methodName, typeName, sourceFile, settings, uniquenessReceived, uniquenessVerified, methodParameters);
        return new(sourceFile, settings, fileConvention);
    }

    SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task<VerifyResult>> verify)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = BuildVerifier(verifySettings, sourceFile);
                return await verify(verifier);
            });
    }
}