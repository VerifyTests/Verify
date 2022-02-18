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
    {
        Extension = extension;
        Name = Path.GetFileName(prefix);
        ReceivedPath = $"{prefix}.received.{extension}";
        VerifiedPath = $"{prefix}.verified.{extension}";
        ReceivedName = Path.GetFileName(ReceivedPath);
        VerifiedName = Path.GetFileName(VerifiedPath);
        IsText = EmptyFiles.Extensions.IsText(extension);
    }

}