namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyZip(
        string path,
        Func<ZipArchiveEntry, bool>? include,
        object? info,
        FileScrubber? scrubber,
        bool includeStructure)
    {
        using var stream = File.OpenRead(path);
        return await VerifyZip(stream, include, info, scrubber, includeStructure);
    }

    public async Task<VerifyResult> VerifyZip(
        Stream stream,
        Func<ZipArchiveEntry, bool>? include,
        object? info,
        FileScrubber? scrubber,
        bool includeStructure)
    {
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        return await VerifyZip(archive, include, info, scrubber, includeStructure);
    }

    public async Task<VerifyResult> VerifyZip(
        byte[] bytes,
        Func<ZipArchiveEntry, bool>? include,
        object? info,
        FileScrubber? scrubber,
        bool includeStructure)
    {
        using var stream = new MemoryStream(bytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        return await VerifyZip(archive, include, info, scrubber, includeStructure);
    }

    public async Task<VerifyResult> VerifyZip(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include,
        object? info,
        FileScrubber? scrubber,
        bool includeStructure)
    {
        var targets = new List<Target>();
        if (info is not null)
        {
            targets.Add(
                new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        info)));
        }

        include ??= _ => true;

        if (includeStructure)
        {
            targets.Add(new("md", ZipStructure.Build(archive), "structure"));
        }

        foreach (var entry in archive.Entries)
        {
            if (!include(entry))
            {
                continue;
            }

            var fullName = entry.FullName;
            if (fullName.EndsWith('/'))
            {
                continue;
            }

            var fileDirectoryPath = Path.GetDirectoryName(fullName)!;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
            var pathWithoutExtension = Path.Combine(fileDirectoryPath, fileNameWithoutExtension);
            // This is a case of file without filename contained inside a directory
            // so let's not mix directory name with filename
            if (fileNameWithoutExtension.Length == 0 &&
                pathWithoutExtension.Length != 0)
            {
                pathWithoutExtension += Path.DirectorySeparatorChar;
            }

            targets.Add(await TargetFromFile(fullName, pathWithoutExtension, scrubber, () => entry.Open()));
        }

        return await VerifyInner(targets);
    }
}