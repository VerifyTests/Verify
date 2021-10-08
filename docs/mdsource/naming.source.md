# File Naming

Naming determines the file name for the `.received.` resulting `.verified.` files.

The format is

```
{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}
```


## Directory

The directory that contains the test. A custom directory can be used via `UseDirectory`:

snippet: UseDirectory

snippet: UseDirectoryFluent

Will result in `CustomDirectory/TypeName.MethodName.verified.txt`.

The path provided can be absolute or relative to the directory that contains the test.


## TestClassName

The class name that contains the test. A custom test name can be used via `UseTypeName`:

snippet: UseTypeName

snippet: UseTypeNameFluent

Will result in `CustomTypeName.MethodName.verified.txt`.


## TestMethodName

The test method name. A custom test name can be used via `UseMethodName`:

snippet: UseMethodName

Will result in `TestClass.CustomMethodName.verified.txt`.

snippet: UseMethodNameFluent

Will result in `TestClass.CustomMethodNameFluent.verified.txt`.


## UseFileName

To fully control the `{TestClassName}.{TestMethodName}_{Parameters}` parts of the file use `UseFileName`:

snippet: UseFileName

Will result in `CustomFileName.verified.txt`.

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


### XUnit

snippet: UniqueForSampleXunit


### NUnit

snippet: UniqueForSampleNUnit


### Expecto

snippet: UniqueForSampleExpecto


### MSTest

snippet: UniqueForSampleMSTest


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

Result in two files:

snippet: Verify.Xunit.Tests/Snippets/ExtensionSample.SharedClassLevelSettings.verified.json

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

For example to place all `.verified.` files in a `{ProjectDirectory}\Snapshots` the following could be used:

snippet: DerivePathInfo

Return null to any of the values to use the standard behavior. The returned path can be relative to the directory sourceFile exists in.

`DerivePathInfo` can also be useful when deriving the storage directory on a [build server](build-server.md#custom-directory-and-file-name)

A `DerivePathInfo` convention can be shipped as a NuGet, for example [Spectre.Verify.Extensions](https://github.com/spectresystems/spectre.verify.extensions) which adds an attribute driven file naming convention to Verify.


### Default DerivePathInfo

snippet: defaultDerivePathInfo