## TempFile

A temporary file helper for tests that automatically cleans up files.


### Features

- Creates unique temporary files in `%TEMP%\VerifyTempFiles\{Path.GetRandomFileName()}`
- Automatic cleanup on dispose
- Implicit conversion to `string` and `FileInfo`
- Thread-safe file creation
- Removes orphaned files older than 24 hours


### Usage

snippet: TempFile


### Orphaned files

Orphaned files can occur in the following scenario

 * A breakpoint is set in a test that uses TempFile
 * Debugger is launched and that breakpoint is hit
 * Debugger is force stopped, resulting in the `TempFile.Dispose()` not being executed


### Path Property

Contains the full path to the temporary file.

snippet: TempFilePathProperty


### Implicit Conversion

Implicit Conversion is helpful as it allows a `TempFile` instance to be passed to directly to method that takes a `string` or a `FileInfo`.


#### String Implicit Conversion

`TempFile` can be implicitly converted to a `string`:

snippet: TempFileStringConversion


#### FileInfo Implicit Conversion

`TempFile` can be implicitly converted to a `FileInfo`:

snippet: TempFileFileInfoConversion


### Info Property

`TempFile` has a convenience `Info` that can be used to access all the `FileInfo` members:

snippet: TempFileInfoProperty


### TempFile RootDirectory Property

Allows access to the root directory for all TempFile instances:

snippet: TempFileRootDirectory


### Cleanup Behavior

The dispose cleans up the current instance.

The static constructor automatically:

1. Ensures the root directory `%TEMP%\VerifyTempFiles` exists
1. Deletes files not modified in the last 24 hours
1. Runs once per application domain


### Thread Safety

Each instance creates a unique file using `Path.GetRandomFileName()`, making concurrent usage safe.


### VerifyFile

`TempFile` is compatible with [VerifyFile](/docs/verify-file.md).

snippet: VerifyTempFile


### TempFile paths are scrubbed

snippet: TempFileScrubbing

Result:

snippet: TempFileTests.Scrubbing.verified.txt


### Debugging

Given `TempFile` deletes the file on test completion (even failure), it can be difficult to debug what caused the failure.

There are several approaches that can be used to inspect the contents of the temp file.

**The below should be considered temporary approaches to be used only during debugging. The code should not be committed to source control.**


#### No Using

Omitting the `using` for the TempFile will prevent the temp file from being deleted when the test finished.

snippet: TempFileNoUsing

The file can then be manually inspected.


#### OpenExplorerAndDebug

Opens the temporary file in the system file explorer and breaks into the debugger.

snippet: TempFileOpenExplorerAndDebug

This method is designed to help debug tests by enabling the inspection of the contents of the temporary file while the test is paused. It performs two actions:

 1. **Opens the file in the file explorer** - Launches the system's default file explorer (Explorer on Windows, Finder on macOS) and navigates to the temporary file
 1. **Breaks into the debugger** - If a debugger is already attached, execution breaks at this point. If no debugger is attached, it attempts to launch one. This prevents the file being clean up by the `TempFile.Dispose()`.

This enables examination of the file contents at a specific point during test execution.

Supported Platforms:

 * Windows (uses `explorer.exe`)
 * macOS (uses `open`)

Throws an exception if used on a build server. Uses `DiffEngine.BuildServerDetector.Detected`.


##### Rider

For `Debugger.Launch();` to work correctly in JetBrains Rider use [Set Rider as the default debugger](https://www.jetbrains.com/help/rider/Settings_Debugger.html#dotNet).