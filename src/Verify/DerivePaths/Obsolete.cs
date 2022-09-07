// ReSharper disable UnusedParameter.Local

namespace VerifyTests;

public static partial class VerifierSettings
{
    [Obsolete("Use Verifier.DerivePathInfo for xunit, expecto and nunit. Use VerifierBase.DerivePathInfo for mstest.")]
    public static void DerivePathInfo()
    {
    }
}