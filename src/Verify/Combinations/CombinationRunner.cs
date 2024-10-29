partial class CombinationRunner
{
    Type[] keyTypes;
    int[] indices;
    object?[][] lists;
    bool captureExceptions;

    public CombinationRunner(bool? captureExceptions, List<IEnumerable<object?>> lists, Type[] keyTypes)
    {
        this.keyTypes = keyTypes;
        this.captureExceptions = captureExceptions ?? CombinationSettings.CaptureExceptionsEnabled;
        this.lists = lists.Select(_ => _.ToArray()).ToArray();
        indices = new int[lists.Count];
    }

    object?[] BuildParameters()
    {
        var parameters = new object?[lists.Length];
        for (var i = 0; i < lists.Length; i++)
        {
            var list = lists[i];
            parameters[i] = list[indices[i]];
        }

        return parameters;
    }

    bool Increment()
    {
        var incrementIndex = lists.Length - 1;
        while (incrementIndex >= 0 &&
               ++indices[incrementIndex] >= lists[incrementIndex].Length)
        {
            indices[incrementIndex] = 0;
            incrementIndex--;
        }

        return incrementIndex < 0;
    }
}