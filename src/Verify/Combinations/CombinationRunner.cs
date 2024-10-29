class CombinationRunner(List<List<object?>> lists, Action<object?[]> action)
{
    int[] indices = new int[lists.Count];

    public void Run()
    {
        while (true)
        {
            var parameters = BuildParameters();

            action(parameters);

            if (Increment())
            {
                break;
            }
        }
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