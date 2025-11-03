## TempDirectory

A temporary directory helper for tests that automatically cleans up directories.


### Features

- Creates unique temporary directories in `%TEMP%\VerifyTempDirectory\{Path.GetRandomFileName()}`
- Automatic cleanup on dispose
- Implicit conversion to `string` and `DirectoryInfo`
- Thread-safe directory creation
- Removes orphaned directories older than 24 hours


### Usage

snippet: TempDirectory


### Orphaned directories

Orphaned directories can occur in the following scenario

 * A breakpoint is set in a test that uses TempDirectory
 * Debugger is launched and that breakpoint is hit
 * Debugger is force stopped, resulting in the `TempDirectory.Dispose()` not being executed


### Path Property

Contains the full path to the temporary directory.

snippet: TempDirectoryPathProperty


### Implicit Conversion

Implicit Conversion is helpful as it allows a `TempDirectory` instance to be passed to directly to method that takes a `string` or a `DirectoryInfo`.


#### String Implicit Conversion

`TempDirectory` can be implicitly converted to a `string`:

snippet: TempDirectoryStringConversion


#### DirectoryInfo Implicit Conversion

`TempDirectory` can be implicitly converted to a `DirectoryInfo`:

snippet: TempDirectoryDirectoryInfoConversion


### Info Property

`TempDirectory` has a convenience `Info` that can be used to access all the `DirectoryInfo` members:

snippet: TempDirectoryInfoProperty


### TempDirectory RootDirectory Property

Allows access to the root directory for all TempDirectory instances:

snippet: TempDirectoryInfoProperty


### Cleanup Behavior

The dispose cleans up the current instance.

The static constructor automatically:

1. Ensures the root directory `%TEMP%\VerifyTempDirectory` exists
1. Deletes sub-directories not modified in the last 24 hours
1. Runs once per application domain


### Thread Safety

Each instance creates a unique directory using `Path.GetRandomFileName()`, making concurrent usage safe.


### VerifyDirectory

`TempDirectory` is compatible with [VerifyDirectory](/docs/verify-directory.md).

snippet: VerifyTempDirectory


### BuildPath

Combines the `TempDirectory.Path` with more paths via Path.Combine.

snippet: TempDirectoryBuildPath


### TempDirectory paths are scrubbed

snippet: TempDirectoryScrubbing

Result:

snippet: C:\Code\Verify\src\Verify.Tests\TempDirectoryTests.Scrubbing.verified.txt


### Debugging

Given `TempDirectory` deletes its contents on test completion (even failure), it can be difficult to debug what caused the failure.

There are several approaches that can be used to inspect the contents of the temp directory.

**The below should be considered temporary approaches to be used only during debugging. The code should not be committed to source control.**


#### No Using

Omitting the `using` for the TempDirectory will prevent the temp directory from being deleted when the test finished.

snippet: TempDirectoryNoUsing

The directory can then be manually inspected.


#### OpenExplorerAndDebug

Opens the temporary directory in the system file explorer and breaks into the debugger.

snippet: TempDirectoryOpenExplorerAndDebug

This method is designed to help debug tests by enabling the inspection of the contents of the temporary directory while the test is paused. It performs two actions:

 1. **Opens the directory in the file explorer** - Launches the system's default file explorer (Explorer on Windows, Finder on macOS) and navigates to the temporary directory
 1. **Breaks into the debugger** - If a debugger is already attached, execution breaks at this point. If no debugger is attached, it attempts to launch one. This prevents the directory being clean up by the `TempDirectory.Dispose()`.

This enables examination of the directory contents at a specific point during test execution.

Supported Platforms:

 * Windows (uses `explorer.exe`)
 * macOS (uses `open`)

Throws an exception if used on a build server. Uses `DiffEngine.BuildServerDetector.Detected`.


##### Rider

For `Debugger.Launch();` to work correctly in JetBrains Rider use [Set Rider as the default debugger](https://www.jetbrains.com/help/rider/Settings_Debugger.html#dotNet).