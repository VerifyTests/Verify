#if DEBUG
[UsesVerify]
public class EncodingTests :
    BaseTest
{
    // [Fact]
    // public async Task UseUtf8NoBomNotAccepted()
    // {
    //     VerifierSettings.UseUtf8NoBom();
    //
    //     var file = CurrentFile.Relative($"EncodingTests.{nameof(UseUtf8NoBomNotAccepted)}.verified.txt");
    //     File.Delete(file);
    //     try
    //     {
    //         await Verify("value");
    //     }
    //     catch
    //     {
    //     }
    //
    //     Assert.Equal(0, new FileInfo(file).Length);
    // }

    [Fact]
    public async Task UseUtf8NoBom()
    {
        VerifierSettings.UseUtf8NoBom();

        var file = CurrentFile.Relative($"EncodingTests.{nameof(UseUtf8NoBom)}.verified.txt");
        File.Delete(file);
        var str = "value";
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        await Verify(str)
            .AutoVerify();
        await Task.Delay(1000);
        var fileBytes = await File.ReadAllBytesAsync(file);
        var expectedBytes = encoding.GetBytes(str);
        Assert.Equal(expectedBytes, fileBytes);
    }

    [Fact]
    public async Task Utf16()
    {
        var encoding = new UnicodeEncoding(
            bigEndian: false,
            byteOrderMark: true,
            throwOnInvalidBytes: true);
        VerifierSettings.UseEncoding(encoding);

        var file = CurrentFile.Relative($"EncodingTests.{nameof(Utf16)}.verified.txt");
        File.Delete(file);
        var str = "value";
        await Verify(str)
            .AutoVerify();
        await Task.Delay(1000);
        var fileBytes = await File.ReadAllBytesAsync(file);
        var expectedBytes = encoding
            .GetPreamble()
            .Concat(encoding.GetBytes(str))
            .ToArray();
        Assert.Equal(expectedBytes, fileBytes);
    }
}
#endif