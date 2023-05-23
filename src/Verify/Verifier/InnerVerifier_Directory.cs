namespace VerifyTests;

partial class InnerVerifier
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

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
        Func<string, Stream> openStream = File.OpenRead;
        var targets = new List<Target>(1);
        if (info is not null)
        {
            targets.Add(new(
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
            var relativePath = pathWithoutExtension[directoryPath.Length..].TrimStart(Path.DirectorySeparatorChar);

            // This is a case of file without filename contained inside a directory
            // so let's not mix directory name with filename
            if (fileNameWithoutExtension.Length == 0 && relativePath.Length != 0)
            {
                relativePath += Path.DirectorySeparatorChar;
            }

            if (!TryGetExtension(path, out var extension))
            {
                targets.Add(new(
                    "noextension",
                    openStream(path),
                    relativePath));
                continue;
            }

            if (FileExtensions.IsText(extension))
            {
                using var stream = openStream(path);
                var builder = await stream.ReadStringBuilderWithFixedLines();
                fileScrubber?.Invoke(path, builder);
                targets.Add(new(
                    extension,
                    builder,
                    relativePath));
                continue;
            }

            targets.Add(new(
                extension,
                openStream(path),
                relativePath));
        }

        return targets;
    }

    static bool TryGetExtension(string path, [NotNullWhen(true)] out string? extension)
    {
        extension = Path.GetExtension(path).Replace(".", string.Empty);
        return extension.Length > 0;
    }
}