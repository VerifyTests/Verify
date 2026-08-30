class State
{
    ConcurrentQueue<ToAppend> items = [];

    internal IReadOnlyCollection<ToAppend> Items => items;

    public bool Paused { get; private set; }

    public void Add(string name, object item)
    {
        Ensure.NotNullOrEmpty(name);
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

    /// <summary>
    /// Set once the recording has been consumed. Nulling the AsyncLocal does not flow back
    /// to the caller when the engine stops a recording from inside the verification, so the
    /// stop has to be observable through the caller's own reference to this state.
    /// </summary>
    public bool Stopped { get; private set; }

    public void Stop()
    {
        Clear();
        Pause();
        Stopped = true;
    }

    public void Pause() =>
        Paused = true;

    public void Resume() =>
        Paused = false;

    public void Clear() =>
        items.Clear();
}