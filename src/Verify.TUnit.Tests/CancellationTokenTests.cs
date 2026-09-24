public class CancellationTokenTests
{
    [Test]
    [Arguments("a")]
    [Arguments("b")]
    public Task WithCancellationToken(string value, CancellationToken token) =>
        Verify(value);
}
