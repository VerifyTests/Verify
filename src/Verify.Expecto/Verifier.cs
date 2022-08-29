namespace VerifyExpecto;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, string name)
    {
        if (settings.typeName is not null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseTypeName));
        }

        if (settings.methodName is not null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseMethodName));
        }

        if (settings.parameters is not null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseParameters));
        }

        if (settings.parametersText is not null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseTextForParameters));
        }

        var fileName = Path.GetFileNameWithoutExtension(sourceFile);

        GetFileConvention fileConvention = (uniquenessReceived, uniquenessVerified) =>
            ReflectionFileNameBuilder.FileNamePrefix(name, fileName, sourceFile, settings, uniquenessReceived, uniquenessVerified, new());

        return new(sourceFile, settings, fileConvention);
    }

    [DoesNotReturn]
    static void ThrowNotSupported(string api) =>
        throw new($"Expect does not support `{api}()`. Change the `name` parameter instead.");

    static async Task Verify(VerifySettings? settings, Assembly assembly, string sourceFile, string name, Func<InnerVerifier, Task> verify)
    {
        TargetAssembly.Assign(assembly);
        settings ??= new();
        Guard.AgainstBadSourceFile(sourceFile);
        using var verifier = GetVerifier(settings, sourceFile, name);
        await verify(verifier);
    }
}