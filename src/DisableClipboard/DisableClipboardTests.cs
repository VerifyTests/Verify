using DiffEngine;
using TextCopy;

[UsesVerify]
public class DisableClipboardTests
{
    static DisableClipboardTests()
    {
        VerifierSettings.DisableClipboard();
        DiffRunner.Disabled = true;
    }

    [Fact]
    public async Task String()
    {
        try
        {
            await Verifier.Verify("Foo");
        }
        catch
        {
        }

        await AssertClipboardNotEffected();
    }

    static async Task AssertClipboardNotEffected()
    {
        var text = await ClipboardService.GetTextAsync();
        if (text is null)
        {
            return;
        }

        Assert.DoesNotContain("DisableClipboardTests", text);
    }
}