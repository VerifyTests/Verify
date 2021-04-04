class ScopeEntry
{
    public string Message { get; }
    public object? State { get; }

    public ScopeEntry(string message, object? state)
    {
        Message = message;
        State = state;
    }
}