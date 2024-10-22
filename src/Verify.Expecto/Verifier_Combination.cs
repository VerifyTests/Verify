// ReSharper disable InconsistentNaming
namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A>(
        string name,
        Func<A, object?> processCall,
        IEnumerable<A> a,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B>(
        string name,
        Func<A, B, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C>(
        string name,
        Func<A, B, C, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b, c));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D>(
        string name,
        Func<A, B, C, D, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b, c, d));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E>(
        string name,
        Func<A, B, C, D, E, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b, c, d, e));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F>(
        string name,
        Func<A, B, C, D, E, F, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b, c, d, e, f));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G>(
        string name,
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
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b, c, d, e, f, g));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G, H>(
        string name,
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
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, a, b, c, d, e, f, g, h));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations(
        string name,
        Func<object?[], object?> processCall,
        List<IEnumerable<object?>> lists,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(processCall, lists));
}