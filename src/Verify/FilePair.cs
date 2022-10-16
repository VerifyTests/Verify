namespace VerifyTests;

public readonly struct FilePair
{
    public string Extension { get; }
    public string ReceivedPath { get; }
    public string VerifiedPath { get; }
    public bool IsText { get; }

    public FilePair(string extension, string receivedPath, string verifiedPath)
    {
        Extension = extension;
        ReceivedPath = receivedPath;
        VerifiedPath = verifiedPath;
        IsText = FileExtensions.IsText(extension);
    }
}