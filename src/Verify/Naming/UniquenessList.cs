class UniquenessList
{
    List<string> inner;

    public UniquenessList() =>
        inner = [];

    public UniquenessList(UniquenessList value) =>
        inner = [..value.inner];

    public void Add(string value)
    {
        if (!inner.Contains(value))
        {
            inner.Add(value);
        }
    }

    public override string ToString()
    {
        if (inner.Count == 0)
        {
            return string.Empty;
        }

        return '.' + StringPolyfill.Join('.', inner);
    }
}