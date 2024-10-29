partial class CombinationRunner
{
    Type[] keyTypes;
    int[] indices;
    List<List<object?>> lists;
    bool captureExceptions;

    public CombinationRunner(bool? captureExceptions, List<IEnumerable<object?>> lists, Type[] keyTypes)
    {
        this.keyTypes = keyTypes;
        this.captureExceptions = captureExceptions ?? VerifyCombinationSettings.CaptureExceptionsEnabled;
        this.lists = lists.Select(_ => _.ToList()).ToList();
        indices = new int[lists.Count];
    }

    object?[] BuildParameters()
    {
        var parameters = new object?[lists.Count];
        for (var i = 0; i < lists.Count; i++)
        {
            var list = lists[i];
            parameters[i] = list[indices[i]];
        }

        return parameters;
    }

    bool Increment()
    {
        var incrementIndex = lists.Count - 1;
        while (incrementIndex >= 0 &&
               ++indices[incrementIndex] >= lists[incrementIndex].Count)
        {
            indices[incrementIndex] = 0;
            incrementIndex--;
        }

        return incrementIndex < 0;
    }
}