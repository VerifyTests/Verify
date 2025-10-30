#pragma warning disable InnerVerifyChecks
namespace VerifyTUnit;

public static class VerifyChecks
{
    public static Task Run()
    {
        var details = TestContext.Current!.Metadata.TestDetails;
        var type = details.MethodMetadata.Class.Type;
        VerifierSettings.AssignTargetAssembly(type.Assembly);
        return InnerVerifyChecks.Run(type.Assembly);
    }
}