namespace VerifyTests;

public readonly struct FilePair(string extension, string receivedPath, string verifiedPath)
{
    public string Extension { get; } = extension;
    public string ReceivedPath { get; } = receivedPath;
    public string VerifiedPath { get; } = verifiedPath;
    public bool IsText { get; } = FileExtensions.IsTextExtension(extension);
}