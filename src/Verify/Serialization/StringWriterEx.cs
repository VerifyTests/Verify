using System.IO;
using System.Text;

class StringWriterEx :
    StringWriter
{
    bool newLineEscapingDisabled;

    public StringWriterEx(StringBuilder builder, bool newLineEscapingDisabled):
        base(builder)
    {
        this.newLineEscapingDisabled = newLineEscapingDisabled;
        base.NewLine = "\n";
    }

    public override void Write(string? value)
    {
        if (newLineEscapingDisabled && value == "\\n")
        {
            base.Write('\n');
            return;
        }
        if (value == @"\\")
        {
            base.Write('\\');
            return;
        }
        base.Write(value);
    }
}