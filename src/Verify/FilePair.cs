namespace VerifyTests;

public readonly struct FilePair
{
    public string Extension { get; }
    public string ReceivedPath { get; }
    public string VerifiedPath { get; }
    public bool IsText { get; }

    public FilePair(string extension, string prefixReceived, string prefixVerified)
    {
        Extension = extension;
        ReceivedPath = $"{prefixReceived}.received.{extension}";
        VerifiedPath = $"{prefixVerified}.verified.{extension}";
        IsText = EmptyFiles.Extensions.IsText(extension);
    }
}