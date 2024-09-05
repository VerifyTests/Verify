#pragma warning disable InnerVerifyChecks
namespace VerifyFixie;

public static class VerifyChecks
{
    public static Task Run(Assembly assembly) =>
        InnerVerifyChecks.Run(assembly);
}