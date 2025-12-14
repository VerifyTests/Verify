namespace VerifyTests;

partial class InnerVerifier
{
#if NET6_0_OR_GREATER

    public async Task<VerifyResult> VerifyDirectory(
        string path,
        Func<string, bool>? include,
        string? pattern,
        EnumerationOptions? option,
        object? info,
        FileScrubber? fileScrubber)
    {
        Ensure.DirectoryExists(path);
        path = Path.GetFullPath(path);
        pattern ??= "*";
        option ??= new()
        {
            RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.System
        };
        var targets = await ToTargetsForDirectory(
            path,
            include,
            Directory.EnumerateFiles(
                path,
                pattern,
                option),
            fileScrubber);
        return await VerifyInner(info, null, targets, true, true);
    }

#else
    public async Task<VerifyResult> VerifyDirectory(
        string path,
        Func<string, bool>? include,
        string? pattern,
        SearchOption option,
        object? info,
        FileScrubber? fileScrubber)
    {
        Ensure.DirectoryExists(path);
        path = Path.GetFullPath(path);
        pattern ??= "*";
        var targets = await ToTargetsForDirectory(
            path,
            include,
            Directory.EnumerateFiles(
                path,
                pattern,
                option),
            fileScrubber);
        return await VerifyInner(info, null, targets, true, true);
    }

#endif

    async Task<List<Target>> ToTargetsForDirectory(
        string directoryPath,
        Func<string, bool>? include,
        IEnumerable<string> enumerateFiles,
        FileScrubber? fileScrubber)
    {
        var targets = new List<Target>();

        include ??= _ => true;

        foreach (var path in enumerateFiles)
        {
            if (!include(path))
            {
                continue;
            }

            var name = NameForRelativePath(directoryPath, path);

            targets.Add(await TargetFromFile(path, name, fileScrubber, () => File.OpenRead(path)));
        }

        return targets;
    }

    static string NameForRelativePath(string directoryPath, string path)
    {
        var fileDirectoryPath = Path.GetDirectoryName(path)!;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        var pathWithoutExtension = Path.Combine(fileDirectoryPath, fileNameWithoutExtension);
        var relativePath = pathWithoutExtension[directoryPath.Length..]
            .TrimStart(Path.DirectorySeparatorChar);

        // This is a case of file without filename contained inside a directory
        // so let's not mix directory name with filename
        if (fileNameWithoutExtension.Length == 0 &&
            relativePath.Length != 0)
        {
            relativePath += Path.DirectorySeparatorChar;
        }

        return relativePath;
    }

    static async Task<Target> TargetFromFile(string path, string name, FileScrubber? fileScrubber, Func<Stream> openStream)
    {
        var extension = Path
            .GetExtension(path)
            .Replace(".", string.Empty);

        if (extension.Length == 0)
        {
            extension = "noextension";
        }

        if (FileExtensions.IsTextFile(path))
        {
            using var stream = openStream();
            var builder = await stream.ReadStringBuilderWithFixedLines();
            fileScrubber?.Invoke(path, builder);
            return new(
                extension,
                builder,
                name);
        }

        return new(
            extension,
            openStream(),
            name);
    }
}