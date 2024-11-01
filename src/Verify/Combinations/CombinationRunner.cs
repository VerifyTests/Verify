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

    Task<CombinationResults> RunWithReturn<TReturn>(Func<object?[], Task<TReturn>> method) =>
        InnerRun(async keys =>
        {
            object? value = await method(keys);
            var paused = Recording.IsPaused();
            if (Recording.IsRecording() || paused)
            {
                var appends = Recording.Values().ToList();
                value = new InfoBuilder(value, appends);
                Recording.Clear();
                if (paused)
                {
                    Recording.Resume();
                }
            }

            return (CombinationResult.ForValue(keys, value), value);
        });


    Task<CombinationResults> RunWithVoid(Func<object?[], Task> method) =>
        InnerRun(async keys =>
        {
            await method(keys);
            if (Recording.IsRecording())
            {
                var appends = Recording.Values().ToList();
                var value = new InfoBuilder("void", appends);
                Recording.Clear();
                return (CombinationResult.ForValue(keys, value), null);
            }
            return (CombinationResult.ForVoid(keys), null);
        });

    async Task<CombinationResults> InnerRun(Func<object?[], Task<(CombinationResult result, object? value)>> method)
    {
        var items = new List<CombinationResult>();
        while (true)
        {
            var keys = BuildParameters();
            try
            {
                await CombinationSettings.RunBeforeCallbacks(keys);
                var (result, value) = await method(keys);
                await CombinationSettings.RunAfterCallbacks(keys, value);
                items.Add(result);
            }
            catch (Exception exception)
                when (captureExceptions)
            {
                await CombinationSettings.RunExceptionCallbacks(keys, exception);
                items.Add(CombinationResult.ForException(keys, exception));
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