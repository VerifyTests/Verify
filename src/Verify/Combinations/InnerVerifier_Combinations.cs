// ReSharper disable InconsistentNaming
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a)
    {
        var (lists, keyTypes) = KeyTypes.Build(a);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B>(
        Func<A, B, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b, c);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b, c, d);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b, c, d, e);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b, c, d, e, f);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b, c, d, e, f, g);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h)
    {
        var (lists, keyTypes) = KeyTypes.Build(a, b, c, d, e, f, g, h);
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            lists,
            keyTypes);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations(
        Func<object?[], object?> method,
        bool? captureExceptions,
        List<IEnumerable<object?>> lists)
    {
        var keyTypes = KeyTypes.Build(lists);
        var target = RunCombinations(method, captureExceptions, lists, keyTypes);
        return Verify(target);
    }

    static CombinationResults RunCombinations(
        Func<object?[], object?> method,
        bool? captureExceptions,
        List<IEnumerable<object?>> lists,
        Type[] keyTypes)
    {
        var listCopy = lists.Select(_ => _.ToList()).ToList();
        var items = new List<CombinationResult>();

        var resolvedCaptureException = captureExceptions ?? VerifyCombinationSettings.CaptureExceptionsEnabled;

        var combinationGenerator = new CombinationRunner(
            listCopy,
            keys =>
            {
                object? value;
                try
                {
                    value = method(keys);
                }
                catch (TargetInvocationException exception)
                    when (resolvedCaptureException)
                {
                    items.Add(new(keys, exception.InnerException!));
                    return;
                }
                catch (Exception exception)
                    when (resolvedCaptureException)
                {
                    items.Add(new(keys, exception));
                    return;
                }

                items.Add(new(keys, value));
            });
        combinationGenerator.Run();
        return new(items, keyTypes);
    }
}