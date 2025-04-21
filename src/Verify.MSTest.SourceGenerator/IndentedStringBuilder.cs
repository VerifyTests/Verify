/// <summary>
/// A <see cref="StringBuilder"/> wrapper that automatically adds indentation to the beginning of
/// every line.
/// </summary>
/// <remarks>
/// Source generators must target netstandard2.0, thus we can't use any newer features related to
/// interpolated string handling without incurring additional allocations for the formatted strings.
///
/// As a result, all methods are chainable to make building up lines easier.
/// </remarks>
class IndentedStringBuilder
{
    StringBuilder builder = new(4096);
    int indentLevel;
    bool isIndented;

    public IndentedStringBuilder IncreaseIndent()
    {
        indentLevel += 1;

        return this;
    }

    public IndentedStringBuilder DecreaseIndent()
    {
        indentLevel -= 1;

        return this;
    }

    public IndentedStringBuilder AppendLine()
    {
        builder.AppendLine();
        isIndented = false;
        return this;
    }

    public IndentedStringBuilder AppendLine(string line)
    {
        WriteIndentIfNeeded();
        builder.AppendLine(line);
        isIndented = false;
        return this;
    }

    public IndentedStringBuilder Append(string text)
    {
        WriteIndentIfNeeded();
        builder.Append(text);
        isIndented = true;
        return this;
    }

    public IndentedStringBuilder Append(IEnumerable<string> strings)
    {
        WriteIndentIfNeeded();
        foreach (var s in strings)
        {
            builder.Append(s);
        }

        isIndented = true;
        return this;
    }

    public IndentedStringBuilder Clear()
    {
        indentLevel = 0;
        builder.Clear();
        isIndented = false;
        return this;
    }

    public override string ToString() => builder.ToString();

    void WriteIndentIfNeeded()
    {
        if (!isIndented)
        {
            builder.Append(' ', indentLevel * 4);
        }
    }
}