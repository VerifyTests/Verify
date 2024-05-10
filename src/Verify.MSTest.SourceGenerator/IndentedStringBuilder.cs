namespace Verify.MSTest.SourceGenerator;

class IndentedStringBuilder
{
    private readonly StringBuilder builder = new StringBuilder();
    private int indentLevel = 0;

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

    public IndentedStringBuilder AppendLine(string line)
    {
        builder.Append(' ', indentLevel * 4);
        builder.AppendLine(line);

        return this;
    }

    public IndentedStringBuilder AppendLine(IEnumerable<string> strings)
    {
        builder.Append(' ', indentLevel * 4);
        foreach (var s in strings)
        {
            builder.Append(s);
        }
        builder.AppendLine();

        return this;
    }

    public IndentedStringBuilder Append(string text)
    {
        builder.Append(' ', indentLevel * 4);
        builder.Append(text);

        return this;
    }

    public IndentedStringBuilder Append(IEnumerable<string> strings)
    {
        builder.Append(' ', indentLevel * 4);
        foreach (var s in strings)
        {
            builder.Append(s);
        }

        return this;
    }

    public override string ToString() => builder.ToString();
}
