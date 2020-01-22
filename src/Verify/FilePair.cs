using System.Diagnostics;
using System.IO;

[DebuggerDisplay("Extension = {Extension} | Name = {FileName}")]
class FilePair
{
    public string Extension { get; }
    public string Received { get; }
    public string ReceivedFileName => Path.GetFileName(Received);
    public string Verified { get; }
    public string FileName => Path.GetFileName(Verified.Replace(".verified."+Extension, ""));

    public FilePair(string extension, string received, string verified)
    {
        Extension = extension;
        Received = received;
        Verified = verified;
    }
}