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

snippet: UniqueForSample

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

snippet: ExtensionSample.cs

Result in two files:

snippet: ExtensionSample.SharedClassLevelSettings.verified.json

snippet: ExtensionSample.AtMethod.verified.xml