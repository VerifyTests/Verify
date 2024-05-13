namespace Verify.MSTest.SourceGenerator;

class IndentedStringBuilder
{
    // TODO: Tweak default capacity based on real-world usage
    private const int DefaultStringBuilderCapacity = 1024;
    private readonly StringBuilder builder;
    private int indentLevel = 0;
    private bool isIndented = false;

    public IndentedStringBuilder(int capacity = DefaultStringBuilderCapacity) =>
        builder = new StringBuilder(capacity);

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

    private void WriteIndentIfNeeded()
    {
        if (!isIndented)
        {
            builder.Append(' ', indentLevel * 4);
        }
    }
}
