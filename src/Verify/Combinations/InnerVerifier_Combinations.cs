// ReSharper disable InconsistentNaming
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a)
    {
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [a.Cast<object?>()],
            [
                typeof(A)
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B>(
        Func<A, B, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B)
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> method,
        bool? captureExceptions,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C)
            ]);
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
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D)
            ]);
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
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E)
            ]);
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
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E),
                typeof(F)
            ]);
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
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E),
                typeof(F),
                typeof(G)
            ]);
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
        var target = RunCombinations(
            method.DynamicInvoke,
            captureExceptions,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>(),
                h.Cast<object?>()
            ],
            [
                typeof(A),
                typeof(B),
                typeof(C),
                typeof(D),
                typeof(E),
                typeof(F),
                typeof(G),
                typeof(H)
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations(
        Func<object?[], object?> method,
        bool? captureExceptions,
        List<IEnumerable<object?>> lists)
    {
        var target = RunCombinations(method, captureExceptions, lists, null);
        return Verify(target);
    }

    static CombinationResults RunCombinations(
        Func<object?[], object?> method,
        bool? captureExceptions,
        List<IEnumerable<object?>> lists,
        Type[]? keyTypes)
    {
        var items = new List<CombinationResult>();
        var listCopy = lists.Select(_ => _.ToList()).ToList();
        keyTypes = BuildKeyTypes(lists, keyTypes);

        var resolvedCaptureException = captureExceptions ?? VerifyCombinationSettings.CaptureExceptionsEnabled;

        var combinationGenerator = new CombinationRunner(
            listCopy,
            combo =>
            {
                var keys = combo.ToArray();
                object? value;
                try
                {
                    value = method(combo);
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

    static Type[] BuildKeyTypes(List<IEnumerable<object?>> lists, Type[]? types)
    {
        if (types != null)
        {
            return types;
        }

        types = new Type[lists.Count];
        for (var index = 0; index < lists.Count; index++)
        {
            var keys = lists[index];
            Type? type = null;
            foreach (var key in keys)
            {
                if (key == null)
                {
                    continue;
                }

                var current = key.GetType();
                if (type == null)
                {
                    type = current;
                    continue;
                }

                if (type != current)
                {
                    type = null;
                    break;
                }

                type = current;
            }

            types[index] = type ?? typeof(object);
        }

        return types;
    }
}