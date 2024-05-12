namespace Verify.MSTest.SourceGenerator;

class IndentedStringBuilder
{
    // TODO: Tweak default capacity based on real-world usage
    private const int DefaultStringBuilderCapacity = 1024;
    private readonly StringBuilder builder;
    private int indentLevel = 0;

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
        return this;
    }

    public IndentedStringBuilder AppendLine(string line, bool indent = true)
    {
        WriteIndentIfNeeded(indent);
        builder.AppendLine(line);
        return this;
    }

    public IndentedStringBuilder Append(string text, bool indent = true)
    {
        WriteIndentIfNeeded(indent);
        builder.Append(text);

        return this;
    }

    public IndentedStringBuilder Append(IEnumerable<string> strings, bool indent = true)
    {
        WriteIndentIfNeeded(indent);
        foreach (var s in strings)
        {
            builder.Append(s);
        }

        return this;
    }

    public IndentedStringBuilder Clear()
    {
        indentLevel = 0;
        builder.Clear();

        return this;
    }

    public override string ToString() => builder.ToString();

    private void WriteIndentIfNeeded(bool indent)
    {
        if (indent)
        {
            builder.Append(' ', indentLevel * 4);
        }
    }
}
