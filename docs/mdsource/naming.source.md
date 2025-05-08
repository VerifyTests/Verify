# File Naming

Naming determines the file name for the `.received.` resulting `.verified.` files.

The format is

```
{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}
```


## Directory

The directory that contains the test.

The path provided can be absolute or relative to the directory that contains the test.


### Instance

snippet: UseDirectory


### Fluent

snippet: UseDirectoryFluent

Will result in `CustomDirectory/TypeName.MethodName.verified.txt`.


## TestClassName

The class name that contains the test. A custom test name can be used via `UseTypeName`:


### Instance

snippet: UseTypeName


### Fluent

snippet: UseTypeNameFluent

Will result in `CustomTypeName.MethodName.verified.txt`.


## TestMethodName

The test method name. A custom test name can be used via `UseMethodName`:

snippet: UseMethodName

Will result in `TestClass.CustomMethodName.verified.txt`.


### Fluent

snippet: UseMethodNameFluent

Will result in `TestClass.CustomMethodNameFluent.verified.txt`.


### Multiple calls to Verify

`UseMethodName` can also be used to allow multiple calls to Verify in the same method:

snippet: MultipleCalls


## UseFileName

To fully control the `{TestClassName}.{TestMethodName}_{Parameters}` parts of the file use `UseFileName`:


### Instance

snippet: UseFileName

Will result in `CustomFileName.verified.txt`.


### Fluent

snippet: UseFileNameFluent

Will result in `UseFileNameFluent.verified.txt`.

Compatibility:

 * Not compatible with `UseTypeName`, `UseMethodName`, or `UseParameters`. An exception will be thrown if they are combined.
 * Can be used in combination with `UseDirectory`.
 * Can be used in combination with `UniqueFor*`.


## Parameters

See [Parameterised Tests](parameterised.md).


## UniqueFor

`UniqueFor*` allows for one or more delimiters to be added to the file name.


### NUnit

snippet: UniqueForSampleNUnit


### XUnit

snippet: UniqueForSampleXunit


### Fixie

snippet: UniqueForSampleFixie


### MSTest

snippet: UniqueForSampleMSTest


### Expecto

snippet: UniqueForSampleExpecto


### TUnit

snippet: UniqueForSampleTUnit


### Result

For a project executed on both x64 and x86 that targets

```
<TargetFrameworks>netcoreapp3.0;net48</TargetFrameworks>
```

Will result in the following files being produced:

```
UniqueForSample.Runtime.Core.verified.txt
UniqueForSample.Runtime.Net.verified.txt
UniqueForSample.RuntimeAndVersion.Core3_0.verified.txt
UniqueForSample.RuntimeAndVersion.Net4_8.verified.txt
UniqueForSample.Architecture.X86.verified.txt
UniqueForSample.Architecture.X64.verified.txt
```


## Extension

The default file extension is `.txt`. So the resulting verified file will be `TestClass.TestMethod.verified.txt`.

It can be overridden at two levels:

 * Method: Change the extension for the current test method.
 * Class: Change the extension all verifications in all test methods for a test class.

Usage:

snippet: XunitExtensionSample

Result in:

snippet: Verify.Xunit.Tests/Snippets/ExtensionSample.AtMethod.verified.xml


## NamerRuntimeAndVersion

To access the current Namer `Runtime` or `RuntimeAndVersion` strings use:

snippet: AccessNamerRuntimeAndVersion


## DerivePathInfo

`DerivePathInfo` allows the storage directory of `.verified.` files to be customized based on the current context. The contextual parameters are parameters passed are as follows:

 * `sourceFile`: The full path to the file that the test existed in at compile time.
 * `projectDirectory`: The directory that the project existed in at compile time.
 * `type`: The class the test method exists in.
 * `method`: The test method.

Any value provided in [UseDirectory](#directory) will take precedence over the value provided in `PathInfo.Directory`.

Return null to any of the values to use the standard behavior. The returned path can be relative to the directory sourceFile exists in.

`DerivePathInfo` can also be useful when deriving the storage directory on a [build server](build-server.md#custom-directory-and-file-name)

For example to place all `.verified.` files in a `{ProjectDirectory}\Snapshots` the following could be used:


### Xunit

snippet: DerivePathInfoXUnitV3


### NUnit

snippet: DerivePathInfoNunit


### MSTest

snippet: DerivePathInfoMSTest


### Expecto

snippet: DerivePathInfoExpecto


### As a nuget

A `DerivePathInfo` convention can be shipped as a NuGet, for example [Spectre.Verify.Extensions](https://github.com/spectresystems/spectre.verify.extensions) which adds an attribute driven file naming convention to Verify.


### Default DerivePathInfo

snippet: defaultDerivePathInfo

Where `NameWithParent` is

snippet: NameWithParent

Any path calculated in `DerivePathInfo` should be fully qualified to remove the inconsistency of the current directory.


### UseProjectRelativeDirectory

`Verifier.UseProjectRelativeDirectory` is a wrapper around `DerivePathInfo` that stores all `.verified.` files in a directory relative to the project directory.

For example to place all `.verified.` files in a `{ProjectDirectory}\Snapshots` the following could be used:

```
Verifier.UseProjectRelativeDirectory("Snapshots");
```


### UseSourceFileRelativeDirectory

`Verifier.UseSourceFileRelativeDirectory` is a wrapper around `DerivePathInfo` that stores all `.verified.` files in a directory relative to the source file directory.

For example to place all `.verified.` files in a `{SourceFileDirectory}\Snapshots` the following could be used:

```
Verifier.UseSourceFileRelativeDirectory("Snapshots");
```


## DisableRequireUniquePrefix

Snapshot file names have to be unique. If a duplicate name is used, then an exception will be throw. This is mostly caused by a conflicting combination of `Verifier.DerivePathInfo()`, `UseMethodName.UseDirectory()`, `UseMethodName.UseTypeName()`, and `UseMethodName.UseMethodName()`. If that's not the case, and having multiple identical prefixes is acceptable, then call `VerifierSettings.DisableRequireUniquePrefix()` to disable this uniqueness validation


## UseUniqueDirectory

An alternative to the "unique file name in the current test directory".

This approach uses "a unique directory in the current test directory".

Useful when many test produce many files, and it is desirable to have them grouped in a directory.

The file format is:

```
{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}/{targetName}.verified.{extension}
```

snippet: UseUniqueDirectory


### UseSplitModeForUniqueDirectory

`UseSplitModeForUniqueDirectory` is a global option that changes the behavior of all `UseUniqueDirectory` uses.

The `received` and `verified` are split and each exist in their own directory.

The file format is:

For received files:

```
{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.received/{targetName}.{extension}
```

For verified files:

```
{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified/{targetName}.{extension}
```

snippet: UseSplitModeForUniqueDirectory

**Also exclude `*.received/` from source control.**

eg. add the following to `.gitignore`

`*.received/`


## Received and multi-targeting

When a test project uses more than one `TargetFrameworks` (eg `<TargetFrameworks>net48;net7.0</TargetFrameworks>`) the runtime and version will be always be added as a uniqueness to the received file. This prevents file locking contention when the tests from both target framework run in parallel.


## Orphaned verified files

One problem with Verify is there is currently no way to track or clean up orphaned verified files.


### Scenario

Given the following test

```
[TestFixture]
public class MyFixture
{
    [Test]
    public Task MyTest1() => Verify("Value");
}
```

The resulting verified file will be `MyFixture.MyTest1.verified.txt`

Now the test is changed to

```
[TestFixture]
public class MyFixture
{
    [Test]
    public Task Test1() => Verify("Value");
}
```

The new resulting verified file will be `MyFixture.Test1.verified.txt`.

The old file, `MyFixture.MyTest1.verified.txt`, will be now orphaned and never be cleaned up.


### Mitigation

For small renames, with resulting small number of orphaned files, the recommended approach is to manually rename the verified files. Or alternatively:

 * Delete the orphaned `*.verified.*` files.
 * Run the test(s)
 * Accept all changes using one of the [Snapshot management](https://github.com/VerifyTests/Verify?tab=readme-ov-file#snapshot-management) approaches.

In some scenarios it may be necessary to clean up many orphaned files. For example from a rename of test fixture with many tests, or a test with many parameter permutations. In this case the delete can be performed by [DiffEngine Tray - Purge verified files
](https://github.com/VerifyTests/DiffEngine/blob/main/docs/tray.md#purge-verified-files) feature.



## UseParametersAppender

Overrides how parameters are appended to verified and received filenames.

The blow sample results in two verified files

 * `Tests.UseParametersAppenderARG1=one_ARG2=two_.verified.txt1
 * `Tests.UseParametersAppenderFluentARG1=three_ARG2=four_.verified.txt`


### Instance

snippet: UseParametersAppender


### Fluent

snippet: UseParametersAppenderFluent