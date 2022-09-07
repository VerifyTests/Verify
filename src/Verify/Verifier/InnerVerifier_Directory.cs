partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include)
    {
        Guard.DirectoryExists(path, nameof(path));
        var targets = await GetTargets(path, include?? (_ => true) ).ToList();
        return await VerifyInner(null, null, targets);
    }

    static async IAsyncEnumerable<Target> GetTargets(string path, Func<string, bool> include)
    {
        foreach (var file in Directory.EnumerateFiles(path))
        {
            if (!include(file))
            {
                continue;
            }

            var name = Path.GetFileNameWithoutExtension(file);
            var extension = Path.GetExtension(file)[1..];
            if (EmptyFiles.Extensions.IsText(extension))
            {
                yield return new(
                    extension,
                    await IoHelpers.ReadStringWithFixedLines(file),
                    name);
            }
            else
            {
                yield return new(
                    extension,
                    File.OpenRead(file),
                    name);
            }
        }
    }

    public Task<VerifyResult> VerifyDirectory(DirectoryInfo target, Func<string, bool>? include) =>
        VerifyDirectory(target.FullName, include);
}