namespace VerifyTests;

partial class InnerVerifier
{
#if NET5_0_OR_GREATER

    public async Task<VerifyResult> VerifyDirectory(
        string path,
        Func<string, bool>? include,
        string? pattern,
        EnumerationOptions? option,
        object? info,
        FileScrubber? fileScrubber)
    {
        Guard.DirectoryExists(path);
        path = Path.GetFullPath(path);
        pattern ??= "*";
        option ??= new()
        {
            RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.System
        };
        var targets = await ToTargets(
            path,
            include,
            Directory.EnumerateFiles(
                path,
                pattern,
                option),
            info,
            fileScrubber);
        return await VerifyInner(targets);
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
        Guard.DirectoryExists(path);
        path = Path.GetFullPath(path);
        pattern ??= "*";
        var targets = await ToTargets(
            path,
            include,
            Directory.EnumerateFiles(
                path,
                pattern,
                option),
            info,
            fileScrubber);
        return await VerifyInner(targets);
    }

#endif

    async Task<List<Target>> ToTargets(
        string directoryPath,
        Func<string, bool>? include,
        IEnumerable<string> enumerateFiles,
        object? info,
        FileScrubber? fileScrubber)
    {
        var targets = new List<Target>(1);
        if (info is not null)
        {
            targets.Add(
                new(
                    VerifierSettings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        info)));
        }

        include ??= _ => true;

        foreach (var path in enumerateFiles)
        {
            if (!include(path))
            {
                continue;
            }

            var fileDirectoryPath = Path.GetDirectoryName(path)!;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var pathWithoutExtension = Path.Combine(fileDirectoryPath, fileNameWithoutExtension);
            var relativePath = pathWithoutExtension[directoryPath.Length..]
                .TrimStart(Path.DirectorySeparatorChar);

            // This is a case of file without filename contained inside a directory
            // so let's not mix directory name with filename
            if (fileNameWithoutExtension.Length == 0 && relativePath.Length != 0)
            {
                relativePath += Path.DirectorySeparatorChar;
            }

            targets.Add(await TargetFromFile(path, relativePath, fileScrubber, () => File.OpenRead(path)));
        }

        return targets;
    }

    static async Task<Target> TargetFromFile(string path, string relativePath, FileScrubber? fileScrubber, Func<Stream> openStream)
    {
        if (!TryGetExtension(path, out var extension))
        {
            return new(
                "noextension",
                openStream(),
                relativePath);
        }

        if (FileExtensions.IsTextExtension(extension))
        {
            using var stream = openStream();
            var builder = await stream.ReadStringBuilderWithFixedLines();
            fileScrubber?.Invoke(path, builder);
            return new(
                extension,
                builder,
                relativePath);
        }

        return new(
            extension,
            openStream(),
            relativePath);
    }

    static bool TryGetExtension(string path, [NotNullWhen(true)] out string? extension)
    {
        extension = Path
            .GetExtension(path)
            .Replace(".", string.Empty);
        return extension.Length > 0;
    }
}