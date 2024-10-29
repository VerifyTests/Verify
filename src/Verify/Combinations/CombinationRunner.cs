class CombinationRunner
{
    int[] indices;
    List<List<object?>> lists;
    bool captureExceptions;

    public CombinationRunner(bool? captureExceptions, List<List<object?>> lists)
    {
        this.captureExceptions = captureExceptions ?? VerifyCombinationSettings.CaptureExceptionsEnabled;
        this.lists = lists;
        indices = new int[lists.Count];
    }

    public CombinationResults Run(Func<object?[], object?> method, Type[] keyTypes)
    {
        var items = new List<CombinationResult>();
        while (true)
        {
            var keys = BuildParameters();
            try
            {
                var value = method(keys);
                items.Add(new(keys, value));
            }
            catch (TargetInvocationException exception)
                when (captureExceptions)
            {
                items.Add(new(keys, exception.InnerException!));
            }
            catch (Exception exception)
                when (captureExceptions)
            {
                items.Add(new(keys, exception));
            }

            if (Increment())
            {
                break;
            }
        }
        return new(items, keyTypes);
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