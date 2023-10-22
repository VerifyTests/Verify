class State
{
    List<ToAppend> items = new();
    bool paused;

    internal IReadOnlyCollection<ToAppend> Items => items;

    public void Add(string name, object item)
    {
        if (paused)
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
        paused = true;

    public void Resume() =>
        paused = false;

    public void Clear()
    {
        lock (items)
        {
            items.Clear();
        }
    }
}