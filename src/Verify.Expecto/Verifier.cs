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

        return new(
            sourceFile,
            settings,
            (uniquenessReceived, uniquenessVerified) =>
            {
                var directory = settings.Directory ?? Path.GetDirectoryName(sourceFile)!;
                var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                return (
                    receivedPrefix: $"{fileName}.{name}{uniquenessReceived}",
                    verifiedPrefix: $"{fileName}.{name}{uniquenessVerified}",
                    directory);
            });
    }

    [DoesNotReturn]
    static void ThrowNotSupported(string api) =>
        throw new($"Expect does not support `{api}()`. Change the `name` parameter instead.");

    static async Task<VerifyResult> Verify(VerifySettings? settings, Assembly assembly, string sourceFile, string name, Func<InnerVerifier, Task<VerifyResult>> verify)
    {
        TargetAssembly.Assign(assembly);
        settings ??= new();
        Guard.AgainstBadSourceFile(sourceFile);
        using var verifier = GetVerifier(settings, sourceFile, name);
        return await verify(verifier);
    }
}