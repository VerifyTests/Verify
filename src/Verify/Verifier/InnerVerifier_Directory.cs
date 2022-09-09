partial class InnerVerifier
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include, string? pattern, EnumerationOptions? option)
    {
        Guard.DirectoryExists(path);
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
                    option))
            .ToList();
        return await VerifyInner(null, null, targets);
    }

#else

    public async Task<VerifyResult> VerifyDirectory(string path, Func<string, bool>? include, string? pattern, SearchOption option)
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
                    option))
            .ToList();
        return await VerifyInner(null, null, targets);
    }

#endif

    static async IAsyncEnumerable<Target> ToTargets(string path, Func<string, bool>? include, IEnumerable<string> enumerateFiles)
    {
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

            var indexOfPeriod = file.LastIndexOf('.');
            var name = file;
            if (indexOfPeriod > -1)
            {
                name = file[..^(file.Length - indexOfPeriod)];
            }

            name = name[path.Length..];
            name = name.TrimStart(Path.DirectorySeparatorChar);

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
}