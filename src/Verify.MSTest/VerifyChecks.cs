#pragma warning disable InnerVerifyChecks
namespace VerifyMSTest;

public static class VerifyChecks
{
    public static Task Run()
    {
        var context = Verifier.GetTestContext();
        return InnerVerifyChecks.Run(context.Assembly);
    }
}