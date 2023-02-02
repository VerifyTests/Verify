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

    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include, string? pattern, SearchOption option, object? info, FileScrubber? fileScrubber)
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

    async Task<List<Target>> ToTargets(string directoryPath, Func<string, bool>? include, IEnumerable<string> enumerateFiles, object? info, FileScrubber? fileScrubber)
    {
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

        foreach (var filePath in enumerateFiles)
        {
            if (!include(filePath))
            {
                continue;
            }

            var extension = Path.GetExtension(filePath).Replace(".", string.Empty);
            var fileDirectoryPath = Path.GetDirectoryName(filePath)!;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var pathWithoutExtension = Path.Combine(fileDirectoryPath, fileNameWithoutExtension);
            var relativePath = pathWithoutExtension[directoryPath.Length..].TrimStart(Path.DirectorySeparatorChar);

            //This is a case of file without filename contained inside a directory - so let's not mix directory name with filename
            if (string.IsNullOrEmpty(fileNameWithoutExtension) && !string.IsNullOrEmpty(relativePath))
            {
                relativePath += Path.DirectorySeparatorChar;
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = "noextension";
            }

            if (FileExtensions.IsText(extension))
            {
                var builder = await IoHelpers.ReadStringBuilderWithFixedLines(filePath);
                fileScrubber?.Invoke(filePath, builder);
                targets.Add(new(
                    extension,
                    builder,
                    relativePath));
            }
            else
            {
                targets.Add(new(
                    extension,
                    File.OpenRead(filePath),
                    relativePath));
            }
        }

        return targets;
    }
}