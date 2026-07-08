public class VerifyXmlStreamTests
{
    [Fact]
    public async Task DisposesStream()
    {
        var stream = new MemoryStream("<root />"u8.ToArray());
        await VerifyXml(stream);
        // VerifyXml(Stream) must dispose the stream, like the other stream APIs.
        Assert.False(stream.CanRead);
    }
}
