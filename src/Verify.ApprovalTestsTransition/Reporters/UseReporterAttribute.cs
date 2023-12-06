// ReSharper disable UnusedParameter.Local

namespace ApprovalTests.Reporters;

[Obsolete("Verify does not use reporters. For Clipboard settings see https://github.com/VerifyTests/Verify/blob/main/docs/clipboard.md. " + DiffReporter.Error, true)]
[AttributeUsage(AttributeTargets.All)]
public class UseReporterAttribute : Attribute
{
    public UseReporterAttribute(params Type[] reporters)
    {
    }
}