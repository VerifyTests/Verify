namespace ApprovalTests.Reporters.ContinuousIntegration;

[Obsolete(Error, true)]
public static class ContinuousDeliveryUtils
{
    public const string Error = "CI reporters are not required in Verify. Instead CI is automatically detected https://github.com/VerifyTests/DiffEngine#buildserverdetector";
}