## TempDirectory

A temporary directory helper for tests that automatically cleans up directories.


### Features

- Creates unique temporary directories in `%TEMP%\VerifyTempDirectory\{Path.GetRandomFileName()}`
- Automatic cleanup on dispose
- Removes orphaned directories older than 24 hours
- Implicit conversion to `string` and `DirectoryInfo`
- Thread-safe directory creation

### Usage

snippet: TempDirectory


### Cleanup Behavior

The dispose cleans up the current instance.

The static constructor automatically:

1. Creates root directory at `%TEMP%\VerifyTempDirectory`
1. Deletes sub-directories not modified in the last 24 hours
1. Runs once per application domain


### Thread Safety

Each instance creates a unique directory using `Path.GetRandomFileName()`, making concurrent usage safe.