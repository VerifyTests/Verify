# File Naming

Naming determines the file name for the `.received.` resulting `.verified.` files.

The format is

```
{UniqueTestName}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`
```

## UniqueTestName

The file prefix uses [XunitContext UniqueTestName](https://github.com/SimonCropp/XunitContext#uniquetestname).


## UniqueFor

UniqueFor allows for one or more delimiters to be added to the file name.


### XUnit

snippet: UniqueForSampleXunit


### NUnit

snippet: UniqueForSampleNUnit


### MSTest

snippet: UniqueForSampleMSTest


### Result

For a project that targets

```
<TargetFrameworks>netcoreapp3.0;net48</TargetFrameworks>
```

Will result in the following files being produced:

```
UniqueForSample.Runtime.Core.verified.txt
UniqueForSample.Runtime.Net.verified.txt
UniqueForSample.RuntimeAndVersion.Core3_0.verified.txt
UniqueForSample.RuntimeAndVersion.Net4_8.verified.txt
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


## DeriveTestDirectory

DeriveTestDirectory allows the storage directory of `.verified.` files to be customized based on the current context. The contextual parameters are parameters passed are as follows:

 * `sourceFile`: The full path to the file that the test existed in at compile time.
 * `projectDirectory`: The directory that the project existed in at compile time.

Foe example to place all `.verified.` files in a `{ProjectDirectory}\Snapshots` the following could be used:

snippet: DeriveTestDirectory

DeriveTestDirectory can also be useful when deriving the storage directory on a [build server](build-server.md#custom-Test-directory)