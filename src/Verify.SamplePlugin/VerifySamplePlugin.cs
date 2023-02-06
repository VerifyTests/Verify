namespace VerifyTests;

public static class VerifySamplePlugin
{
    public static bool Initialized { get; private set; }

    public static void Initialize() =>
        Initialized = true;
}