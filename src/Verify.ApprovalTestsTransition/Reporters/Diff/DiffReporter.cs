namespace ApprovalTests.Reporters;

[Obsolete(Error, true)]
public class DiffReporter
{
    public const string Error = "DiffReporters are not required in Verify. Instead the best diff tool is automatically selected. Diff tools can also be configured. See https://github.com/VerifyTests/DiffEngine/blob/main/docs/diff-tool.order.md";
}