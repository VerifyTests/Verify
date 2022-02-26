namespace VerifyTests;

[DebuggerDisplay("Extension = {Extension} | Name = {Name}")]
public readonly struct FilePair
{
    public string Extension { get; }
    public string ReceivedPath { get; }
    public string VerifiedPath { get; }
    public string ReceivedName { get; }
    public string VerifiedName { get; }
    public string Name { get; }
    public bool IsText { get; }

    public FilePair(string extension, string prefix) 
        : this (extension, prefix, prefix)
    {
    }

    public FilePair(string extension, string prefixReceived, string prefixVerified)
    {
        Extension = extension;
        Name = Path.GetFileName(prefixReceived);
        ReceivedPath = $"{prefixReceived}.received.{extension}";
        VerifiedPath = $"{prefixVerified}.verified.{extension}";
        ReceivedName = Path.GetFileName(ReceivedPath);
        VerifiedName = Path.GetFileName(VerifiedPath);
        IsText = EmptyFiles.Extensions.IsText(extension);
    }
}