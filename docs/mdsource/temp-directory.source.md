## TempDirectory

A temporary directory helper for tests that automatically cleans up directories.


### Features

- Creates unique temporary directories in `%TEMP%\VerifyTempDirectory\{Path.GetRandomFileName()}`
- Automatic cleanup on dispose
- Implicit conversion to `string` and `DirectoryInfo`
- Thread-safe directory creation
- Removes orphaned directories older than 24 hours


#### Orphaned directories

Orphaned directories can occur in the following scenario

 * A breakpoint is set in a test that uses TempDirectory
 * Debugger is launched and that breakpoint is hit
 * Debugger is force stopped, resulting in the `TempDirectory.Dispose()` not being executed


### Usage

snippet: TempDirectory


### Path Property

Contains the full path to the temporary directory.

snippet: TempDirectoryPathProperty


### Implicit Conversion

Implicit Conversion is helpful as it allows a `TempDirectory` instance to be passed to directly to method that takes a `string` or a `DirectoryInfo`.


#### String Implicit Conversion

`TempDirectory` can be implicitly converter to a `string`:

snippet: TempDirectoryStringConversion


#### DirectoryInfo Implicit Conversion

`TempDirectory` can be implicitly converter to a `DirectoryInfo`:

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


### Debugging

Given `TempDirectory` deletes its contents on test completion (even failure), it can be difficult to debug what caused the failure.


#### Rider

For `Debugger.Launch();` to work correctly in JetBrains Rider use [Set Rider as the default debugger](https://www.jetbrains.com/help/rider/Settings_Debugger.html#dotNet).