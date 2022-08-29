﻿namespace VerifyExpecto;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, string methodName)
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

        return new(sourceFile, settings, fileName, methodName, new());
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