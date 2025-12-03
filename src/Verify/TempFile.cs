using IoPath = System.IO.Path;

namespace VerifyTests;

/// <summary>
/// Provides a temporary file that is automatically cleaned up when disposed.
/// The class maintains a shared root directory and automatically deletes files older than 24 hours.
/// </summary>
/// <remarks>
/// <para>
/// This class is designed for use in tests where temporary file storage is needed.
/// Each instance creates a unique file under a common root location.
/// </para>
/// <para>
/// On first use (static initialization), the class performs automatic cleanup of any
/// files in the root that haven't been modified in over 24 hours. This helps
/// prevent accumulation of orphaned test files from previous test runs.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Basic usage
/// using var temp = new TempFile();
/// File.WriteAllText(temp, "content");
/// // File is automatically deleted when disposed
///
/// // With extension
/// using var txtFile = new TempFile(".txt");
/// File.WriteAllText(txtFile, "content");
/// </code>
/// </example>
public class TempFile :
    IDisposable
{
    List<string> paths;

    /// <summary>
    /// The full path to the file.
    /// </summary>
    public string Path { get; }

    public override string ToString() => Path;

    /// <summary>
    /// Gets the root directory path where all temporary files are created.
    /// </summary>
    /// <value>
    /// The full path to the root directory, located at
    /// <c>[System.Temp]\VerifyTempFiles</c>.
    /// </value>
    /// <remarks>
    /// This directory is created during static initialization if it doesn't exist.
    /// All temporary files created by instances of this class are stored in this location.
    /// </remarks>
    public static string RootDirectory { get; } = IoPath.Combine(IoPath.GetTempPath(), "VerifyTempFiles");

    [ModuleInitializer]
    public static void Init()
    {
        Directory.CreateDirectory(RootDirectory);

        VerifierSettings.OnVerify(
            before: () =>
            {
            },
            after: () => asyncPaths.Value = null);

        VerifierSettings.GlobalScrubbers.Add((scrubber, _, _) =>
        {
            var pathsValue = asyncPaths.Value;
            if (pathsValue == null)
            {
                return;
            }

            foreach (var path in pathsValue)
            {
                scrubber.Replace(path, "{TempFile}");
            }
        });

        Cleanup();
    }

    static AsyncLocal<List<string>?> asyncPaths = new();

    internal static void Cleanup()
    {
        var cutoffTime = DateTime.Now.AddDays(-1);

        foreach (var file in Directory.EnumerateFiles(RootDirectory))
        {
            var fileInfo = new FileInfo(file);

            if (!fileInfo.Exists || fileInfo.LastWriteTime >= cutoffTime)
            {
                continue;
            }

            try
            {
                File.Delete(file);
            }
            catch (FileNotFoundException)
            {
                // Ignore file cleanup race condition
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TempFile"/> class.
    /// </summary>
    /// <param name="extension">
    /// Optional file extension (e.g., ".txt", ".json").
    /// If not provided, no extension is added.
    /// The extension should include the leading dot.
    /// </param>
    /// <remarks>
    /// Each instance creates a unique file under <see cref="RootDirectory"/>
    /// using <see cref="Path.GetRandomFileName"/> to generate a random name.
    /// The file is created immediately upon construction as an empty file.
    /// </remarks>
    /// <exception cref="IOException">
    /// Thrown if the file cannot be created (e.g., due to permissions or disk space).
    /// </exception>
    public TempFile(string? extension = null)
    {
        Ensure.NotEmpty(extension);
        var fileName = IoPath.GetRandomFileName();
        if (extension != null)
        {
            fileName = IoPath.ChangeExtension(fileName, extension);
        }

        Path = IoPath.Combine(RootDirectory, fileName);

        paths = asyncPaths.Value!;
        if (paths == null)
        {
            paths = asyncPaths.Value = [Path];
        }
        else
        {
            paths.Add(Path);
        }
    }

    /// <summary>
    /// Creates a new temporary file with optional extension and encoding.
    /// </summary>
    /// <param name="extension">
    /// Optional file extension (e.g., ".txt", ".json").
    /// If not provided, no extension is added.
    /// The extension should include the leading dot.
    /// </param>
    /// <param name="encoding">
    /// Optional text encoding to use when creating the file.
    /// If provided and the extension is recognized as a text format, the file will be created with the appropriate BOM.
    /// If null, the file is created as an empty binary file.
    /// </param>
    /// <returns>
    /// A new <see cref="TempFile"/> instance representing the created file.
    /// The caller is responsible for disposing the instance to ensure cleanup.
    /// </returns>
    /// <remarks>
    /// The file is created immediately upon calling this method. If encoding is specified and the extension
    /// is recognized as a text format, the file will be initialized with the appropriate byte order mark (BOM).
    /// Otherwise, an empty file is created.
    /// </remarks>
    /// <exception cref="IOException">
    /// Thrown if the file cannot be created (e.g., due to permissions or disk space).
    /// </exception>
    /// <example>
    /// <code>
    /// // Create empty temp file
    /// using var temp = TempFile.Create();
    ///
    /// // Create temp file with extension
    /// using var txtFile = TempFile.Create(".txt");
    ///
    /// // Create temp file with UTF-8 encoding and BOM
    /// using var utf8File = TempFile.Create(".txt", Encoding.UTF8);
    /// </code>
    /// </example>
    public static TempFile Create(string? extension = null, Encoding? encoding = null)
    {
        var file = new TempFile(extension);

        if (extension == null ||
            !AllFiles.TryCreateFile(file, true, encoding))
        {
            File.Create(file).Dispose();
        }

        return file;
    }

    /// <summary>
    /// Creates a new temporary file with the specified text content.
    /// </summary>
    /// <param name="content">
    /// The text content to write to the file.
    /// </param>
    /// <param name="extension">
    /// Optional file extension (e.g., ".txt", ".json", ".xml").
    /// If not provided, no extension is added.
    /// The extension should include the leading dot.
    /// </param>
    /// <param name="encoding">
    /// Optional text encoding to use when writing the content.
    /// If null, the default UTF-8 encoding without BOM is used.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a new <see cref="TempFile"/> instance with the written content.
    /// The caller is responsible for disposing the instance to ensure cleanup.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The file is created and written asynchronously. The content is written using the specified
    /// encoding, or UTF-8 without BOM if no encoding is specified.
    /// </para>
    /// <para>
    /// This method is ideal for creating temporary test data files, configuration files, or
    /// any text-based content that needs to be written to disk for testing purposes.
    /// </para>
    /// </remarks>
    /// <exception cref="IOException">
    /// Thrown if the file cannot be created or written to (e.g., due to permissions or disk space).
    /// </exception>
    /// <example>
    /// <code>
    /// // Create temp file with simple text
    /// using var temp = await TempFile.CreateText("Hello, World!");
    ///
    /// // Create JSON file
    /// using var json = await TempFile.CreateText(
    ///     "{\"name\": \"test\"}",
    ///     ".json");
    ///
    /// // Create file with specific encoding
    /// using var utf8 = await TempFile.CreateText(
    ///     "Content with special chars: äöü",
    ///     ".txt",
    ///     Encoding.UTF8);
    /// </code>
    /// </example>
    public static async Task<TempFile> CreateText(string content, string? extension = null, Encoding? encoding = null)
    {
        var file = new TempFile(extension);

        if (encoding == null)
        {
            await File.WriteAllTextAsync(file, content);
        }
        else
        {
            await File.WriteAllTextAsync(file, content, encoding);
        }

        return file;
    }

    /// <summary>
    /// Creates a new temporary file with the specified binary content.
    /// </summary>
    /// <param name="content">
    /// The binary content to write to the file.
    /// Can be empty to create an empty file.
    /// </param>
    /// <param name="extension">
    /// Optional file extension (e.g., ".bin", ".dat", ".png").
    /// If not provided, no extension is added.
    /// The extension should include the leading dot.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a new <see cref="TempFile"/> instance with the written content.
    /// The caller is responsible for disposing the instance to ensure cleanup.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The file is created and written asynchronously with the exact binary content provided.
    /// This method is suitable for creating temporary files containing binary data such as
    /// images, serialized objects, or any non-text data.
    /// </para>
    /// <para>
    /// The content is written as-is without any encoding or transformation.
    /// </para>
    /// </remarks>
    /// <exception cref="IOException">
    /// Thrown if the file cannot be created or written to (e.g., due to permissions or disk space).
    /// </exception>
    /// <example>
    /// <code>
    /// // Create temp file with binary data
    /// byte[] data = [0x01, 0x02, 0x03, 0x04];
    /// using var temp = await TempFile.CreateBinary(data);
    ///
    /// // Create image file
    /// byte[] imageData = await File.ReadAllBytesAsync("source.png");
    /// using var image = await TempFile.CreateBinary(imageData, ".png");
    ///
    /// // Create empty binary file
    /// using var empty = await TempFile.CreateBinary(
    ///     ReadOnlyMemory&lt;byte&gt;.Empty,
    ///     ".bin");
    /// </code>
    /// </example>
    public static async Task<TempFile> CreateBinary(ReadOnlyMemory<byte> content, string? extension = null)
    {
        var file = new TempFile(extension);

        await File.WriteAllBytesAsync(file, content);

        return file;
    }

    public void Dispose()
    {
        if (File.Exists(Path))
        {
            File.Delete(Path);
        }

        paths.Remove(Path);
    }

    /// <summary>
    /// Opens the temporary file's containing directory in the system file explorer and breaks into the debugger.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is designed to help debug tests by inspecting the temporary file
    /// while the test is paused. It performs two actions:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description>
    /// Opens the file's containing directory in the system's default file explorer
    /// (Explorer on Windows, Finder on macOS), with the file selected.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If a debugger is already attached, it breaks execution at this point.
    /// If no debugger is attached, it attempts to launch one.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Thrown when the method is called on an unsupported operating system.
    /// Currently supports Windows and macOS only.
    /// </exception>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// Thrown if the file explorer process cannot be started.
    /// </exception>
    /// <example>
    /// <code>
    /// [Fact]
    /// public void TestWithInspection()
    /// {
    ///     using var temp = new TempFile(".txt");
    ///     File.WriteAllText(temp, "content");
    ///
    ///     // Pause and inspect the file
    ///     temp.OpenExplorerAndDebug();
    ///
    ///     // Continue with assertions...
    /// }
    /// </code>
    /// </example>
    public void OpenExplorerAndDebug()
    {
        PathLauncher.Launch(Arguments());

        string Arguments()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"/select,\"{Path}\"";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"-R \"{Path}\"";
            }

            throw new($"Unsupported operating system: {RuntimeInformation.OSDescription}");
        }
    }

    /// <summary>
    /// Implicitly converts a <see cref="TempFile"/> to its file path string.
    /// </summary>
    /// <param name="temp">The temporary file instance to convert.</param>
    /// <returns>The full path to the temporary file.</returns>
    /// <example>
    /// <code>
    /// using var temp = new TempFile(".txt");
    /// string path = temp;
    /// File.WriteAllText(path, "content");
    /// </code>
    /// </example>
    public static implicit operator string(TempFile temp) =>
        temp.Path;

    /// <summary>
    /// Implicitly converts a <see cref="TempFile"/> to a <see cref="FileInfo"/> object.
    /// </summary>
    /// <param name="temp">The temporary file instance to convert.</param>
    /// <returns>A <see cref="FileInfo"/> object representing the temporary file.</returns>
    /// <example>
    /// <code>
    /// using var temp = new TempFile(".txt");
    /// FileInfo fileInfo = temp;
    /// var length = fileInfo.Length;
    /// </code>
    /// </example>
    public static implicit operator FileInfo(TempFile temp) =>
        new(temp.Path);

    /// <summary>
    /// A <see cref="FileInfo"/> representing this instance.
    /// </summary>
    /// <example>
    /// <code>
    /// using var temp = new TempFile(".txt");
    /// var length = temp.Info.Length;
    /// </code>
    /// </example>
    public FileInfo Info => new(Path);
}