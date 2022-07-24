namespace VerifyTests;

public class VerifyResult
{
    public VerifyResult(IReadOnlyList<FilePair> files) =>
        Files = files;

    public IReadOnlyList<FilePair> Files { get; }

    public string Text
    {
        get
        {
            var textFiles = Files.Where(_ => _.IsText).ToList();
            if (textFiles.Count == 0)
            {
                throw new("No text files in results");
            }

            if (textFiles.Count > 1)
            {
                throw new("More than one text file in results");
            }

            return File.ReadAllText(textFiles[0].VerifiedPath);
        }
    }
}