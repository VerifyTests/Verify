namespace VerifyTests;

public class VerifyResult
{
    IReadOnlyList<FilePair> files;

    internal VerifyResult(IReadOnlyList<FilePair> files, object? target)
    {
        this.files = files;
        Target = target;
    }

    public Exception Exception
    {
        get
        {
            if (Target is null)
            {
                throw new("Target is null");
            }

            if (Target is Exception exception)
            {
                return exception;
            }

            throw new($"Target is a {Target.GetType()}");
        }
    }

    public object? Target { get; }

    public string Text
    {
        get
        {
            var textFiles = TextFiles.ToList();
            if (textFiles.Count == 0)
            {
                throw new("No text files in results");
            }

            if (textFiles.Count > 1)
            {
                throw new("More than one text file in results");
            }

            return File.ReadAllText(textFiles[0]);
        }
    }

    public IEnumerable<string> TextFiles =>
        files
            .Where(_ => _.IsText)
            .Select(_ => _.VerifiedPath);

    public IEnumerable<string> Files =>
        files.Select(_ => _.VerifiedPath);
}