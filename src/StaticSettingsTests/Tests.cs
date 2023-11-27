// disable all test parallelism to avoid test interaction
using DiffEngine;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace StaticSettingsTests;

[UsesVerify]
public class Tests
{
    public Tests()
    {
        // reset all global settings modified by any test
        VerifierSettings.UseEncoding(IoHelpers.Utf8);
    }

    [Fact]
    public async Task NonStandardEncoding_DisableUtf8Bom()
    {
        var file = CurrentFile.Relative($"Tests.{nameof(NonStandardEncoding_DisableUtf8Bom)}.{Namer.RuntimeAndVersion}.verified.txt");
        File.Delete(file);
        var str = "value";
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        #region UseUtf8NoBom
        VerifierSettings.UseUtf8NoBom();
        #endregion
        await Verify(str)
            .UniqueForRuntimeAndVersion()
            .AutoVerify();
        var fileBytes = File.ReadAllBytes(file);
        var expectedBytes = encoding.GetBytes(str);
        Assert.Equal(expectedBytes, fileBytes);
    }

    [Fact]
    public async Task NonStandardEncoding_Utf16()
    {
        var file = CurrentFile.Relative($"Tests.{nameof(NonStandardEncoding_Utf16)}.{Namer.RuntimeAndVersion}.verified.txt");
        File.Delete(file);
        var str = "value";
        #region UseEncoding
        var encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true);
        VerifierSettings.UseEncoding(encoding);
        #endregion
        await Verify(str)
            .UniqueForRuntimeAndVersion()
            .AutoVerify();
        var fileBytes = File.ReadAllBytes(file);
        var expectedBytes = encoding.GetPreamble().Concat(encoding.GetBytes(str)).ToArray();
        Assert.Equal(expectedBytes, fileBytes);
    }
}