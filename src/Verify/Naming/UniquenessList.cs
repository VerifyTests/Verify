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

#if NET5_0_OR_GREATER
        return '.' + string.Join('.', inner);
#else
        return '.' + string.Join(".", inner);
#endif
    }
}