using StringBuilder = System.Text.StringBuilder;

namespace VerifyXunit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyCombinations<A>(
        Func<A, object> processCall,
        IEnumerable<A> list,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var target = GetCombinationString(
            _ => processCall.DynamicInvoke(_.ToArray()),
            null, [list.Cast<object?>().ToList()]);
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    static StringBuilder GetCombinationString(
        Func<List<object?>, object?> processCall,
        Func<object, string>? resultFormatter,
        List<List<object?>> lists)
    {
        var builder = new StringBuilder();
        var result = new List<object?>();
        GenerateCombinations(
            lists,
            0,
            result,
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
        return builder;
    }

    static void GenerateCombinations(List<List<object?>> lists, int depth, List<object?> currentCombination, Action<List<object?>> processCombination)
    {
        if (depth == lists.Count)
        {
            processCombination(currentCombination);
            return;
        }

        foreach (var item in lists[depth])
        {
            currentCombination.Add(item);
            GenerateCombinations(lists, depth + 1, currentCombination, processCombination);
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }
}