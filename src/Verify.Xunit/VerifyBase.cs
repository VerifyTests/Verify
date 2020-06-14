
namespace VerifyXunit
{
    [ObsoleteEx(
        Message = "Instead add a [UsesVerifyAttribute]` use the static Verifier.Verify().",
        ReplacementTypeOrMember = "Verifier.Verify",
        TreatAsErrorFromVersion = "5.0")]
    public class VerifyBase
    {
    }
}