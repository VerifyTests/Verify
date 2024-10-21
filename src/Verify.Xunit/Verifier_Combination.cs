// ReSharper disable InconsistentNaming
namespace VerifyXunit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyCombinations<A>(
        Func<A, string?> processCall,
        IEnumerable<A> a,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(processCall, a));

    [Pure]
    public static SettingsTask VerifyCombinations<A, B>(
        Func<A, B, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(processCall, a, b));

    [Pure]
    public static SettingsTask VerifyCombinations<A, B, C>(
        Func<A, B, C, string?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.VerifyCombinations(processCall, a, b, c));
}