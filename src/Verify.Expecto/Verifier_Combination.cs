// ReSharper disable InconsistentNaming
// ReSharper disable RedundantSuppressNullableWarningExpression
namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A>(
        string name,
        Func<A, object?> method,
        IEnumerable<A> a,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B>(
        string name,
        Func<A, B, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C>(
        string name,
        Func<A, B, C, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D>(
        string name,
        Func<A, B, C, D, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E>(
        string name,
        Func<A, B, C, D, E, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F>(
        string name,
        Func<A, B, C, D, E, F, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e, f));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G>(
        string name,
        Func<A, B, C, D, E, F, G, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e, f, g));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G, H>(
        string name,
        Func<A, B, C, D, E, F, G, H, object?> method,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, a, b, c, d, e, f, g, h));

    [Pure]
    public static Task<VerifyResult> VerifyCombinations(
        string name,
        Func<object?[], object?> method,
        List<IEnumerable<object?>> lists,
        bool captureExceptions = false,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            Assembly.GetCallingAssembly()!,
            sourceFile,
            name,
            _ => _.VerifyCombinations(method, captureExceptions, lists));
}