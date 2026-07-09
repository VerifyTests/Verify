public class CallbackTests
{
    [Fact]
    public async Task OnFirstVerifyAwaitsAllHandlers()
    {
        var settings = new VerifySettings();
        var ran = new List<int>();
        // The slower handler is registered first: if only the last handler is
        // awaited (the bug), it will not have completed by the assert.
        settings.OnFirstVerify(
            async (_, _, _) =>
            {
                await Task.Delay(200);
                ran.Add(1);
            });
        settings.OnFirstVerify(
            async (_, _, _) =>
            {
                await Task.Delay(1);
                ran.Add(2);
            });

        var filePair = new FilePair("txt", "received.txt", "verified.txt");
        var result = new NewResult(filePair, null);
        await settings.RunOnFirstVerify(result, false);

        Assert.Equal(2, ran.Count);
    }
}
