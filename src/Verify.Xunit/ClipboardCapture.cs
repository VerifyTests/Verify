using System.Text;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new StringBuilder();

    public static void Append(string received, string verified)
    {
        var command = $"cmd /c move /Y \"{received}\" \"{verified}\"";
        lock (builder)
        {
            builder.AppendLine(command);
            Clipboard.SetText(builder.ToString());
        }
    }
}