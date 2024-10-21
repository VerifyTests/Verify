// ReSharper disable InconsistentNaming
namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyCombinations<A>(
        Func<A, object?> processCall,
        IEnumerable<A> a)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [a.Cast<object?>()]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B>(
        Func<A, B, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C>(
        Func<A, B, C, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D>(
        Func<A, B, C, D, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E>(
        Func<A, B, C, D, E, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F>(
        Func<A, B, C, D, E, F, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G>(
        Func<A, B, C, D, E, F, G, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations<A, B, C, D, E, F, G, H>(
        Func<A, B, C, D, E, F, G, H, object?> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        IEnumerable<D> d,
        IEnumerable<E> e,
        IEnumerable<F> f,
        IEnumerable<G> g,
        IEnumerable<H> h)
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>(),
                d.Cast<object?>(),
                e.Cast<object?>(),
                f.Cast<object?>(),
                g.Cast<object?>(),
                h.Cast<object?>()
            ]);
        return Verify(target);
    }

    public Task<VerifyResult> VerifyCombinations(
        Func<object?[], object?> processCall,
        List<IEnumerable<object?>> lists)
    {
        var target = GetCombinationString(processCall, lists);
        return Verify(target);
    }

    static Dictionary<StringBuilder, object?> GetCombinationString(
        Func<object?[], object?> processCall,
        List<IEnumerable<object?>> lists)
    {
        var items = new Dictionary<StringBuilder,object?>();
        var listCopy = lists.Select(_ => _.ToList()).ToList();
        var combinationGenerator = new CombinationGenerator(
            listCopy,
            combo =>
            {
                object? value;
                try
                {
                    value = processCall(combo);
                }
                catch (Exception exception)
                {
                    value = exception;
                }
                items.Add(BuildKeys(combo), value);
            });
        combinationGenerator.Run();
        return items;
    }

    static StringBuilder BuildKeys(object?[] combo)
    {
        var builder = new StringBuilder();
        for (var index = 0; index < combo.Length; index++)
        {
            var item = combo[index];
            VerifierSettings.AppendParameter(item, builder, true);
            if (index + 1 != combo.Length)
            {
                builder.Append(", ");
            }
        }

        return builder;
    }

    public class Item(StringBuilder keys, object? value)
    {
        public StringBuilder Keys { get; } = keys;
        public object? Value { get; } = value;
    }
}

