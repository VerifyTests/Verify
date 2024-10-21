namespace VerifyXunit;

class CombinationGenerator(List<List<object?>> lists, Action<object?[]> action)
{
    object?[] parameters = new object[lists.Count];
    int[] indices = new int[lists.Count];

    public void Run()
    {
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