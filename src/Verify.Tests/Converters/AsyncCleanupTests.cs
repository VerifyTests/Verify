public class AsyncCleanupTests
{
    static bool asyncCleanupRan;

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.RegisterFileConverter<TargetForAsyncCleanup>(
            (instance, _) => new(
                info: null,
                "txt",
                instance.Value,
                cleanup: async () =>
                {
                    await Task.Delay(200);
                    asyncCleanupRan = true;
                }));

    [Fact]
    public async Task ConverterAsyncCleanupIsAwaited()
    {
        asyncCleanupRan = false;
        var target = new TargetForAsyncCleanup("content");
        await Verify(target);
        // The converter's async cleanup must be awaited before Verify returns;
        // composing cleanups with += only awaits the last one.
        Assert.True(asyncCleanupRan);
    }

    public record TargetForAsyncCleanup(string Value);
}
