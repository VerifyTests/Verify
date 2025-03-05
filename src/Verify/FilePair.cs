#pragma warning disable DanglingSnapshotsCheck

namespace VerifyTests;

public readonly struct FilePair
{
    public FilePair(string extension, string receivedPath, string verifiedPath)
    {
        Guards.AgainstBadExtension(extension);
        Extension = extension;
        ReceivedPath = receivedPath;
        VerifiedPath = verifiedPath;
        DanglingSnapshotsCheck.TrackVerifiedFile(verifiedPath);
        IsText = FileExtensions.IsTextExtension(extension);
    }

    public FilePair(string extension, string receivedPath, string verifiedPath, bool isText)
    {
        Guards.AgainstBadExtension(extension);
        Extension = extension;
        ReceivedPath = receivedPath;
        DanglingSnapshotsCheck.TrackVerifiedFile(verifiedPath);
        VerifiedPath = verifiedPath;
        IsText = isText;
    }

    public string Extension { get; }
    public string ReceivedPath { get; }
    public string VerifiedPath { get; }
    public bool IsText { get; }
}