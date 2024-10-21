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
            _ => processCall.DynamicInvoke(_.ToArray()),
            null, [a.Cast<object?>().ToList()]);
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    [Pure]
    public static SettingsTask VerifyCombinations<A,B>(
        Func<A, B, object> processCall,
        IEnumerable<A> a,
        IEnumerable<B> b,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var target = GetCombinationString(
            _ => processCall.DynamicInvoke(_.ToArray()),
            null,
            [
                a.Cast<object?>().ToList(),
                b.Cast<object?>().ToList()
            ]);
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    static StringBuilder GetCombinationString(
        Func<List<object?>, object?> processCall,
        Func<object, string>? resultFormatter,
        List<List<object?>> lists)
    {
        var builder = new StringBuilder();
        var result = new List<object?>();
        var combinationGenerator = new CombinationGenerator(lists,
            combo =>
            {
                builder.Append('[');

                for (var index = 0; index < combo.Count; index++)
                {
                    var item = combo[index];
                    VerifierSettings.AppendParameter(item, builder, true);
                    if (index + 1 != combo.Count)
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

class CombinationGenerator
{
    readonly List<List<object?>> lists;
    readonly Action<List<object?>> action;

    public CombinationGenerator(List<List<object?>> lists, Action<List<object?>> action)
    {
        this.lists = lists;
        this.action = action;
    }
    public void Run()
    {
        var indices = new int[lists.Count];

        while (true)
        {
            var combination = new List<object?>();
            for (var i = 0; i < lists.Count; i++)
            {
                combination.Add(lists[i][indices[i]]);
            }

            action(combination);

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