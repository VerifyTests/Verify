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
        Guard.DirectoryExists(path, nameof(path));
        path = Path.GetFullPath(path);
        pattern ??= "*";
        option ??= new()
        {
            RecurseSubdirectories = true,
            AttributesToSkip = 0
        };
        var targets = await ToTargets(
                path,
                include,
                Directory.EnumerateFiles(
                    path,
                    pattern,
                    option),
                info,
                fileScrubber)
            .ToList();
        return await VerifyInner(targets);
    }

#else

    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include, string? pattern, SearchOption option, object? info, FileScrubber? fileScrubber)
    {
        Guard.DirectoryExists(path, nameof(path));
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
                fileScrubber)
            .ToList();
        return await VerifyInner(targets);
    }

#endif

    async IAsyncEnumerable<Target> ToTargets(string directoryPath, Func<string, bool>? include, IEnumerable<string> enumerateFiles, object? info, FileScrubber? fileScrubber)
    {
        if (info is not null)
        {
            yield return new(
                VerifierSettings.TxtOrJson,
                JsonFormatter.AsJson(
                    settings,
                    counter,
                    info));
        }

        include ??= _ => true;

        foreach (var file in enumerateFiles)
        {
            if (!include(file))
            {
                continue;
            }

            var name = file;

            var extension = Path.GetExtension(file).Replace(".", string.Empty);
            if (string.IsNullOrEmpty(extension))
            {
                extension = "noextension";
            }
            else
            {
                var indexOfPeriod = file.LastIndexOf('.');
                if (indexOfPeriod > -1)
                {
                    name = file[..^(file.Length - indexOfPeriod)];
                }
            }

            name = name[path.Length..];
            name = name.TrimStart(Path.DirectorySeparatorChar);

            name = string.IsNullOrEmpty(name) ? null : name;

            if (FileExtensions.IsText(extension))
            {
                var builder = await IoHelpers.ReadStringBuilderWithFixedLines(file);
                fileScrubber?.Invoke(file, builder);
                yield return new(
                    extension,
                    builder,
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
}