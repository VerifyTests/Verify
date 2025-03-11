# Dangling snapshot files

A dangling snapshot file are when a `.verified.` file exist with no corresponding test. This can occur when a test, with corresponding test snapshot file(s), is renamed or deleted. The snapshot file are now redundant and noise on the file system and in the solution explorer


## How dangling snapshot checks works

 * When each test is executed, any snapshots produced are recorded.
 * After all tests are executed, all recorded snapshots are checked against the snapshots that exist on disk
 * An exception is thrown if any files:
   * exist on disk and do not have a corresponding recorded test
   * have casing that does not match the test case


## Experimental

`DanglingSnapshots` is an experimental feature (marked with `[Experimental("VerifyDanglingSnapshots")]`) and is subject to change in minor version.

To use it requires the  `VerifyDanglingSnapshots` warning to to be disabled using a `pragma warning disable`:

```
#pragma warning disable VerifyDanglingSnapshots
```


## Only in CI

When running tests locally, it is not possible to detect if all tests are being run, or only a subset. As such `DanglingSnapshots.Run()` will only perform checks when executed on a build server.


## Usage

In the tear-down of a test run (after all test have run), call `DanglingSnapshots.Run()`.


### MSTest

Use the `[AssemblyCleanup]` feature:

snippet: DanglingSnapshotsMSTestUsage/DanglingSnapshots.cs


### NUnit

use the `[OneTimeTearDown]` feature:

snippet: DanglingSnapshotsNUnitUsage/DanglingSnapshots.cs


### XUnit

Define an XUnit collection:

snippet: DanglingSnapshotsXUnitUsage/DanglingSnapshots.cs

Apply that collection to all tests:

snippet: XunitDanglingCollection


### XUnitV3

Define an XUnit collection:

snippet: DanglingSnapshotsXUnitV3Usage/DanglingSnapshots.cs

Apply that collection to all tests:

snippet: XunitV3DanglingCollection