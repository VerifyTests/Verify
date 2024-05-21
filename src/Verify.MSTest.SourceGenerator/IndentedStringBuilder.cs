namespace VerifyMSTest.SourceGenerator;

/// <summary>
/// A <see cref="StringBuilder"/> wrapper that automatically adds indentation to the beginning of
/// every line.
/// </summary>
/// <param name="capacity">The initial capacity of the underlying StringBuilder.</param>
/// <remarks>
/// Source generators must target netstandard2.0, thus we can't use any newer features related to
/// interpolated string handling without incurring additional allocations for the formatted strings.
///
/// As a result, all methods are chainable to make building up lines easier.
/// </remarks>
class IndentedStringBuilder(int capacity = IndentedStringBuilder.DefaultStringBuilderCapacity)
{
    // Default capacity based on the closest power of 2 to what's used in our own tests (this value may need to be tweaked over time).
    const int DefaultStringBuilderCapacity = 4096;
    readonly StringBuilder builder = new(capacity);
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