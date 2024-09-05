#pragma warning disable InnerVerifyChecks
namespace VerifyExpecto;

public static class VerifyChecks
{
    public static Task Run(Assembly assembly) =>
        InnerVerifyChecks.Run(assembly);
}