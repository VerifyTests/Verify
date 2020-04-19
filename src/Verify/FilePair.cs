using System.Diagnostics;
using System.IO;

[DebuggerDisplay("Extension = {Extension} | Name = {Name}")]
class FilePair
{
    public string Extension { get; }
    public string Received { get; }
    public string Verified { get; }
    public string Name { get; }

    public FilePair(string extension, string prefix)
    {
        Extension = extension;
        Name = Path.GetFileName(prefix);
        Received = $"{prefix}.received.{extension}";
        Verified = $"{prefix}.verified.{extension}";
    }
}