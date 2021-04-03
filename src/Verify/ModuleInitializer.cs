using System.Runtime.CompilerServices;
using VerifyTests;

static class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.RegisterJsonAppender(_ =>
        {
            if (!LoggerRecording.TryFinishRecording(out var entries))
            {
                return null;
            }

            return new("logs", entries!);
        });
    }
}