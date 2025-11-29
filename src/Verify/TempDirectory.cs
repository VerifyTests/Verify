using IoPath = System.IO.Path;

namespace VerifyTests;

/// <summary>
/// Provides a temporary directory that is automatically cleaned up when disposed.
/// The class maintains a shared root directory and automatically deletes subdirectories older than 24 hours.
/// </summary>
/// <remarks>
/// <para>
/// This class is designed for use in tests where temporary file storage is needed.
/// Each instance creates a unique subdirectory under a common root location.
/// </para>
/// <para>
/// On first use (static initialization), the class performs automatic cleanup of any
/// subdirectories in the root that haven't been modified in over 24 hours. This helps
/// prevent accumulation of orphaned test directories from previous test runs.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Basic usage
/// using var temp = new TempDirectory();
/// File.WriteAllText(Path.Combine(temp, "test.txt"), "content");
/// // Directory is automatically deleted when disposed
/// </code>
/// </example>
public class TempDirectory :
    IDisposable
{
    List<string> paths;

    /// <summary>
    /// The full path to the directory.
    /// </summary>
    public string Path { get; }

    public override string ToString() => Path;

    /// <summary>
    /// Gets the root directory path where all temporary directories are created.
    /// </summary>
    /// <value>
    /// The full path to the root directory, located at
    /// <c>[System.Temp]\VerifyTempDirectory</c>.
    /// </value>
    /// <remarks>
    /// This directory is created during static initialization if it doesn't exist.
    /// All temporary directories created by instances of this class are subdirectories
    /// of this root location.
    /// </remarks>
    public static string RootDirectory { get; } = IoPath.Combine(IoPath.GetTempPath(), "VerifyTempDirectory");

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
                scrubber.Replace(path, "{TempDirectory}");
            }
        });

        Cleanup();
    }

    static AsyncLocal<List<string>?> asyncPaths = new();

    internal static void Cleanup()
    {
        var cutoffTime = DateTime.Now.AddDays(-1);

        foreach (var dir in Directory.EnumerateDirectories(RootDirectory))
        {
            var dirInfo = new DirectoryInfo(dir);

            if (!dirInfo.Exists || dirInfo.LastWriteTime >= cutoffTime)
            {
                continue;
            }
            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch (DirectoryNotFoundException)
            {
                //Ignore directory cleanup race condition
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TempDirectory"/> class.
    /// Creates a new temporary directory with a random name.
    /// </summary>
    /// <remarks>
    /// Each instance creates a unique subdirectory under <see cref="RootDirectory"/>
    /// using <see cref="Path.GetRandomFileName"/> to generate a random name.
    /// The directory is created immediately upon construction.
    /// </remarks>
    /// <exception cref="IOException">
    /// Thrown if the directory cannot be created (e.g., due to permissions or disk space).
    /// </exception>
    public TempDirectory()
    {
        Path = IoPath.Combine(RootDirectory, IoPath.GetRandomFileName());
        Directory.CreateDirectory(Path);

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

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, true);
        }

        paths.Remove(Path);
    }

    /// <summary>
    /// Opens the temporary directory in the system file explorer and breaks into the debugger.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is designed to help debug tests by inspecting the contents
    /// of the temporary directory while the test is paused. It performs two actions:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description>
    /// Opens the temporary directory in the system's default file explorer
    /// (Explorer on Windows, Finder on macOS).
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
    /// Thrown if the file explorer process cannot be started (e.g., the explorer command is not available).
    /// </exception>
    /// <example>
    /// <code>
    /// [Fact]
    /// public void TestWithInspection()
    /// {
    ///     using var temp = new TempDirectory();
    ///
    ///     // Create some test files
    ///     File.WriteAllText(Path.Combine(temp, "test.txt"), "content");
    ///
    ///     // Pause and inspect the directory
    ///     temp.OpenExplorerAndDebug();
    ///
    ///     // Continue with assertions...
    /// }
    /// </code>
    /// </example>
    public void OpenExplorerAndDebug() =>
        PathLauncher.Launch(Path);

    /// <summary>
    /// Implicitly converts a <see cref="TempDirectory"/> to its directory path string.
    /// </summary>
    /// <param name="temp">The temporary directory instance to convert.</param>
    /// <returns>The full path to the temporary directory.</returns>
    /// <example>
    /// <code>
    /// using var temp = new TempDirectory();
    /// string path = temp;
    /// // Prints the directory path
    /// Console.WriteLine(path);
    /// </code>
    /// </example>
    public static implicit operator string(TempDirectory temp) =>
        temp.Path;

    /// <summary>
    /// Implicitly converts a <see cref="TempDirectory"/> to a <see cref="DirectoryInfo"/> object.
    /// </summary>
    /// <param name="temp">The temporary directory instance to convert.</param>
    /// <returns>A <see cref="DirectoryInfo"/> object representing the temporary directory.</returns>
    /// <example>
    /// <code>
    /// using var temp = new TempDirectory();
    /// DirectoryInfo dirInfo = temp;
    /// var files = dirInfo.GetFiles("*.txt");
    /// </code>
    /// </example>
    public static implicit operator DirectoryInfo(TempDirectory temp) =>
        new(temp.Path);

    /// <summary>
    /// A <see cref="DirectoryInfo"/> represeting this instance.
    /// </summary>
    /// <example>
    /// <code>
    /// using var temp = new TempDirectory();
    /// var files = temp.Info.GetFiles("*.txt");
    /// </code>
    /// </example>
    public DirectoryInfo Info => new(Path);

    /// <summary>
    /// Combines the <see cref="Path"/> with <paramref name="paths"/> via Path.Combine.
    /// </summary>
    public string BuildPath(params ReadOnlySpan<string> paths) =>
        IoPath.Combine([Path, ..paths]);
}