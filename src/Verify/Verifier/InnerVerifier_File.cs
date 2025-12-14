namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyFile(string path, object? info = null, string? extension = null)
    {
        Ensure.FileExists(path);
        if (extension == null)
        {
            return VerifyStream(IoHelpers.OpenRead(path), info);
        }

        return VerifyStream(IoHelpers.OpenRead(path), extension, info);
    }

    public Task<VerifyResult> VerifyFile(FileInfo target, object? info = null, string? extension = null) =>
        VerifyFile(target.FullName, info, extension);

    public async Task<VerifyResult> VerifyFiles(
        IEnumerable<string> paths,
        object? info,
        FileScrubber? fileScrubber)
    {
        var targets = await ToTargetsForFiles(paths, fileScrubber);
        return await VerifyInner(info, null, targets, true);
    }

    static async Task<List<Target>> ToTargetsForFiles(
        IEnumerable<string> enumerateFiles,
        FileScrubber? fileScrubber)
    {
        var targets = new List<Target>();

        foreach (var path in enumerateFiles)
        {
            var name = Path.GetFileNameWithoutExtension(path);

            targets.Add(await TargetFromFile(path, name, fileScrubber, () => File.OpenRead(path)));
        }

        return targets;
    }
}