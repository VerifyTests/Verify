namespace Verify.MSTest.SourceGenerator;

class IndentedStringBuilder
{
    private readonly StringBuilder builder = new StringBuilder();
    private int indentLevel = 0;

    public void IncreaseIndent() => indentLevel += 1;

    public void DecreaseIndent() => indentLevel -= 1;

    public void AppendLine(string line)
    {
        builder.Append(' ', indentLevel * 4);
        builder.AppendLine(line);
    }

    public void Append(string text)
    {
        builder.Append(' ', indentLevel * 4);
        builder.Append(text);
    }

    public override string ToString() => builder.ToString();
}
