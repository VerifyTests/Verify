static class IoHelpers
{
    static readonly char[] Separators =
    [
        '\\',
        '/'
    ];

    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static void DeleteFileIfEmpty(string path)
    {
        var info = new FileInfo(path);
        if (info is {Exists: true, Length: 0})
        {
            info.Delete();
        }
    }

    public static void DeleteFiles(string directory, string pattern)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        DeleteFiles(Files(directory, pattern));
    }

    public static IEnumerable<string> Files(string directory, string pattern) =>
        Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories);

    public static void DeleteFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            DeleteFile(file);
        }
    }

    public static void DeleteFile(string file)
    {
        try
        {
            File.Delete(file);
        }
        catch
        {
        }
    }

    [Conditional("DEBUG")]
    public static void ValidateOsPathSeparators(string path, [CallerArgumentExpression("path")] string argumentName = "")
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (path.Contains('/'))
            {
                throw new($"Path '{argumentName}' should not contain / on windows: {path}");
            }
        }
        else
        {
            if (path.Contains('\\'))
            {
                throw new($"Path '{argumentName}' should not contain \\ on mac/linux: {path}");
            }
        }
    }

    public static void CreateDirectory(string directory) =>
        Directory.CreateDirectory(directory);

    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static void ThrowIfEmpty(this Stream stream)
    {
        if (stream.Length == 0)
        {
            throw new("Empty data not supported.");
        }
    }

#if NET6_0_OR_GREATER
    public static async Task DisposeAsyncEx(this Stream stream) =>
        await stream.DisposeAsync();
#else
    public static Task DisposeAsyncEx(this Stream stream)
    {
        stream.Dispose();
        return Task.CompletedTask;
    }
#endif

    static FileStream OpenWrite(string path) =>
        new(path, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize: 4096, useAsync: true);

    public static FileStream OpenRead(string path) =>
        new(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

    public static long Length(string file) =>
        new FileInfo(file).Length;

    public static async Task<StringBuilder> ReadStringBuilderWithFixedLines(this Stream stream)
    {
        stream.MoveToStart();
        using var reader = new StreamReader(stream);
        var builder = new StringBuilder(await reader.ReadToEndAsync());
        builder.FixNewlines();

        return builder;
    }

#if NET472 || NET48
    internal static void WriteText(string path, StringBuilder text)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text.ToString(), VerifierSettings.Encoding);
    }

#else

    internal static void WriteText(string path, StringBuilder text)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        using var writer = new StreamWriter(path, false, VerifierSettings.Encoding);
        writer.Write(text);
    }

#endif

    public static string GetRelativePath(string directory, string file)
    {
        var fullPath = Path.Combine(directory, file);
        var last = directory[^1];
        if (last == Path.DirectorySeparatorChar ||
            last == Path.AltDirectorySeparatorChar)
        {
            return fullPath[directory.Length..];
        }

        return fullPath[(directory.Length + 1)..];
    }

    static VirtualizedRunHelper? virtualizedRunHelper;
    static ConcurrentDictionary<Assembly, VirtualizedRunHelper> virtualizedRunHelpers = [];

    internal static void MapPathsForCallingAssembly(Assembly assembly) =>
        virtualizedRunHelper = GetForAssembly(assembly);

    internal static string GetMappedBuildPath(string path)
    {
        if (virtualizedRunHelper == null)
        {
            return path;
        }

        return virtualizedRunHelper.GetMappedBuildPath(path);
    }

    internal static string GetMappedBuildPath(string path, Assembly assembly)
    {
        var helper = GetForAssembly(assembly);
        return helper.GetMappedBuildPath(path);
    }

    static VirtualizedRunHelper GetForAssembly(Assembly assembly) =>
        virtualizedRunHelpers.GetOrAdd(assembly, _ => new(_));

    /// <summary>
    /// Resolve directory path from a given source file path, this method will remap the path if the .dll was built on a
    /// system (e.g. Windows) and the tests are run on another one (e.g. Linux though WSL or docker)
    /// </summary>
    internal static string ResolveDirectoryFromSourceFile(string sourceFile)
    {
        var mappedFile = GetMappedBuildPath(sourceFile);

        var index = mappedFile.LastIndexOfAny(Separators);
        if (index > 0)
        {
            return mappedFile[..index];
        }

        throw new($"Unable to resolve directory. sourceFile: {sourceFile}");
    }

    public static async Task<StringBuilder> ReadStringBuilderWithFixedLines(string path)
    {
        using var stream = OpenRead(path);
        return await stream.ReadStringBuilderWithFixedLines();
    }

    public static async Task WriteStream(string path, Stream stream)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        if (stream is FileStream fileStream)
        {
            if (fileStream.Length == 0)
            {
                throw new($"Empty data is not allowed. Path: {fileStream.Name}");
            }

            File.Copy(fileStream.Name, path, true);
            return;
        }

        // keep using scope to stream is flushed
        using (var targetStream = OpenWrite(path))
        {
            await stream.SafeCopy(targetStream);
        }

        if (new FileInfo(path).Length == 0)
        {
            throw new($"Empty data is not allowed. Path: {path}");
        }
    }
}