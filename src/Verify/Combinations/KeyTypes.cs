static class KeyTypes
{
    public static Type[] Build(List<IEnumerable<object?>> lists)
    {
        var types = new Type[lists.Count];
        for (var index = 0; index < lists.Count; index++)
        {
            var keys = lists[index];
            Type? type = null;
            foreach (var key in keys)
            {
                if (key == null)
                {
                    continue;
                }

                var current = key.GetType();
                if (type == null)
                {
                    type = current;
                    continue;
                }

                if (type != current)
                {
                    type = null;
                    break;
                }

                type = current;
            }

            types[index] = type ?? typeof(object);
        }

        return types;
    }
}