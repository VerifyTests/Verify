class State
{
    internal ConcurrentBag<ToAppend> Items = new();

    public void Add(string name, object item) =>
        Items.Add(new(name, item));
}