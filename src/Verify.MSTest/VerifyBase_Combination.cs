// ReSharper disable InconsistentNaming
namespace VerifyMSTest;

public partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask VerifyCombinations<A>(
        Func<A, object?> processCall,
        IEnumerable<A> a,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B>(
        Func<A, B, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, c, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, c, d, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, c, d, e, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, c, d, e, f, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, c, d, e, f, g, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, a, b, c, d, e, f, g, h, settings, sourceFile);

    [Pure]
    public SettingsTask VerifyCombinations(
        Func<object?[], object?> processCall,
        List<IEnumerable<object?>> lists,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyCombinations(processCall, lists, settings, sourceFile);
}