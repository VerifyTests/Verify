partial class InnerVerifier
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include, string? pattern, EnumerationOptions? option, object? info)
    {
        Guard.DirectoryExists(path, nameof(path));
        path = Path.GetFullPath(path);
        pattern ??= "*";
        option ??= new()
        {
            RecurseSubdirectories = true
        };
        var targets = await ToTargets(
                path,
                include,
                Directory.EnumerateFiles(
                    path,
                    pattern,
                    option),
                info)
            .ToList();
        return await VerifyInner(targets);
    }

#else

    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include, string? pattern, SearchOption option, object? info)
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
                info)
            .ToList();
        return await VerifyInner(targets);
    }

#endif

    async IAsyncEnumerable<Target> ToTargets(string path, Func<string, bool>? include, IEnumerable<string> enumerateFiles, object? info)
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
        if (include == null)
        {
            include = _ => true;
        }

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

            if (FileExtensions.IsText(extension))
            {
                yield return new(
                    extension,
                    await IoHelpers.ReadStringBuilderWithFixedLines(file),
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