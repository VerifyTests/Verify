using VerifyTests;

namespace VerifyExpecto;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, string name)
    {
        if (settings.typeName != null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseTypeName));
        }

        if (settings.methodName != null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseMethodName));
        }

        if (settings.parameters != null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseParameters));
        }

        if (settings.parametersText != null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseTextForParameters));
        }

        return new(
            sourceFile,
            settings,
            uniqueness =>
            {
                var directory = settings.Directory ?? Path.GetDirectoryName(sourceFile)!;
                var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                return ($"{fileName}.{name}{uniqueness}", directory);
            });
    }

    static void ThrowNotSupported(string api)
    {
        throw new($"Expect does not support `{api}()`. Change the `name` parameter instead.");
    }

    static async Task Verify(VerifySettings? settings, Assembly assembly, string sourceFile, string name, Func<InnerVerifier, Task> verify)
    {
        TargetAssembly.Assign(assembly);
        settings ??= new();
        Guard.AgainstBadSourceFile(sourceFile);
        using var verifier = GetVerifier(settings, sourceFile, name);
        await verify(verifier);
    }
}