class State
{
    ConcurrentQueue<ToAppend> items = [];

    internal IReadOnlyCollection<ToAppend> Items => items;

    public bool Paused { get; private set; }

    public void Add(string name, object item)
    {
        Guard.NotNullOrEmpty(name);
        if (Paused)
        {
            return;
        }

        if (Recording.IsIgnored(name))
        {
            return;
        }

        var append = new ToAppend(name, item);
        items.Enqueue(append);
    }

    public void Pause() =>
        Paused = true;

    public void Resume() =>
        Paused = false;

    public void Clear() =>
        items.Clear();
}