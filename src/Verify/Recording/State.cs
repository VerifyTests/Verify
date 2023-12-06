class State
{
    List<ToAppend> items = [];

    internal IReadOnlyCollection<ToAppend> Items => items;

    public bool Paused { get; private set; }

    public void Add(string name, object item)
    {
        Guard.AgainstNullOrEmpty(name);
        if (Paused)
        {
            return;
        }

        if (Recording.IsIgnored(name))
        {
            return;
        }

        var append = new ToAppend(name, item);
        lock (items)
        {
            items.Add(append);
        }
    }

    public void Pause() =>
        Paused = true;

    public void Resume() =>
        Paused = false;

    public void Clear()
    {
        lock (items)
        {
            items.Clear();
        }
    }
}