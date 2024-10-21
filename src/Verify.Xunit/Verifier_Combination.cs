namespace VerifyXunit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyCombinations<A>(
        Func<A, object> processCall,
        IEnumerable<A> a,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            null,
            [a.Cast<object?>()]);
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    [Pure]
    public static SettingsTask VerifyCombinations<A, B>(
        Func<A, B, object> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            null,
            [
                a.Cast<object?>(),
                b.Cast<object?>()
            ]);
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    [Pure]
    public static SettingsTask VerifyCombinations<A, B, C>(
        Func<A, B, C, object> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        IEnumerable<C> c,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var target = GetCombinationString(
            processCall.DynamicInvoke,
            null,
            [
                a.Cast<object?>(),
                b.Cast<object?>(),
                c.Cast<object?>()
            ]);
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    static StringBuilder GetCombinationString(
        Func<object?[], object?> processCall,
        Func<object, string>? resultFormatter,
        List<IEnumerable<object?>> lists)
    {
        var builder = new StringBuilder();
        var listCopy = lists.Select(_=>_.ToList()).ToList();
        var combinationGenerator = new CombinationGenerator(
            listCopy,
            combo =>
            {
                builder.Append('[');

                for (var index = 0; index < combo.Length; index++)
                {
                    var item = combo[index];
                    VerifierSettings.AppendParameter(item, builder, true);
                    if (index + 1 != combo.Length)
                    {
                        builder.Append(", ");
                    }
                }

                builder.Append("] => ");

                object? result;

                try
                {
                    result = processCall(combo);
                }
                catch (Exception exception)
                {
                    builder.AppendLineN($"Exception: {exception.Message}");
                    return;
                }

                if (result == null)
                {
                    builder.AppendLineN("null");
                    return;
                }

                if (resultFormatter == null)
                {
                    builder.AppendLineN(result.ToString());
                    return;
                }

                builder.AppendLineN(resultFormatter(result));
            });
        combinationGenerator.Run();
        return builder;
    }
}

class CombinationGenerator(List<List<object?>> lists, Action<object?[]> action)
{
    object?[] parameters = new object[lists.Count];

    public void Run()
    {
        var indices = new int[lists.Count];

        while (true)
        {
            for (var i = 0; i < lists.Count; i++)
            {
                var list = lists[i];
                parameters[i] = list[indices[i]];
            }

            action(parameters);

            var incrementIndex = lists.Count - 1;
            while (incrementIndex >= 0 && ++indices[incrementIndex] >= lists[incrementIndex].Count)
            {
                indices[incrementIndex] = 0;
                incrementIndex--;
            }

            if (incrementIndex < 0)
            {
                break;
            }
        }
    }
}