namespace VerifyTests;

public readonly struct FilePair
{
    public FilePair(string extension, string receivedPath, string verifiedPath)
    {
        Guard.AgainstBadExtension(extension);
        Extension = extension;
        ReceivedPath = receivedPath;
        VerifiedPath = verifiedPath;
        IsText = FileExtensions.IsTextExtension(extension);
    }

    public string Extension { get; }
    public string ReceivedPath { get; }
    public string VerifiedPath { get; }
    public bool IsText { get; }
}