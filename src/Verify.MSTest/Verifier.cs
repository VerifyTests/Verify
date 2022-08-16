namespace VerifyMSTest;

public static partial class Verifier
{
    static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings)
    {
        Guard.AgainstBadSourceFile(sourceFile);

        var method = GetTestMethod();
        var type = method.DeclaringType!;
        TargetAssembly.Assign(type.Assembly);

        GetFileConvention fileConvention = (uniquenessForReceived, uniquenessForVerified) =>
            ReflectionFileNameBuilder.FileNamePrefix(method, type, sourceFile, settings, uniquenessForReceived, uniquenessForVerified);

        return new(sourceFile, settings, fileConvention);
    }

    static MethodInfo GetTestMethod()
    {
        var stackTrace = new StackTrace();
        var frames = stackTrace.GetFrames();

        if (frames != null)
        {
            foreach (var stackFrame in frames)
            {
                if (stackFrame!.GetMethod() is MethodInfo method)
                {
                    var testAttribute = method.GetCustomAttribute(typeof(TestMethodAttribute));
                    if (testAttribute != null)
                    {
                        return method!;
                    }
                }
            }
        }

        throw new("Couldn't find test method.");
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