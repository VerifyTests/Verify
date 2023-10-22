class State
{
    List<ToAppend> items = new();

    internal IReadOnlyCollection<ToAppend> Items => items;

    public void Add(string name, object item)
    {
        var append = new ToAppend(name, item);
        lock (items)
        {
            items.Add(append);
        }
    }
}