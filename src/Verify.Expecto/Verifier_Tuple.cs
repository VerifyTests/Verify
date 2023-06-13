// ReSharper disable RedundantSuppressNullableWarningExpression

#if !NETSTANDARD2_0 && !NET462
namespace VerifyExpecto;

public static partial class Verifier
{
    public static Task<VerifyResult> VerifyTuple(
        string name,
        Expression<Func<ITuple>> expression,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyTuple(expression));
    }
}
#endif