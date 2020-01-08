# <img src="/src/icon.png" height="30px"> Verify

[![Build status](https://ci.appveyor.com/api/projects/status/dpqylic0be7s9vnm/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Xunit.svg?label=Verify.Xunit)](https://www.nuget.org/packages/Verify.Xunit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.NUnit.svg?label=Verify.NUnit)](https://www.nuget.org/packages/Verify.NUnit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.MSTest.svg?label=Verify.MSTest)](https://www.nuget.org/packages/Verify.MSTest/)

Verification tool to enable simple approval of complex models and documents.

toc
include: doc-index


## NuGet packages

 * https://nuget.org/packages/Verify.Xunit/
 * https://nuget.org/packages/Verify.NUnit/
 * https://nuget.org/packages/Verify.MSTest/


## Usage


### Class being tested

Given a class to be tested:

snippet: ClassBeingTested


### Test

It can be tested as follows:


#### xUnit

snippet: SampleTestXunit


#### NUnit

snippet: SampleTestNUnit


#### MSTest

snippet: SampleTestMSTest


### Initial Verification

When the test is initially run will fail with:

```
First verification. SampleTest.Simple.verified.txt not found.
Verification command has been copied to the clipboard.
```

The clipboard will contain the following:

> move /Y "C:\Code\Sample\SampleTest.Simple.received.txt" "C:\Code\Sample\SampleTest.Simple.verified.txt"

If a [Diff Tool](docs/diff-tool.md) is detected it will display the diff:

![InitialDiff](/docs/InitialDiff.png)

To verify the result:

 * Execute the command from the clipboard, or
 * Use the diff tool to accept the changes, or
 * Manually copy the text to the new file

This will result in the `SampleTest.Simple.verified.txt` being created:

snippet: Verify.Xunit.Tests/Snippets/SampleTest.Simple.verified.txt


### Subsequent Verification

If the implementation of `ClassBeingTested` changes:

snippet: ClassBeingTestedChanged

And the test is re run it will fail with

```
Verification command has been copied to the clipboard.
Assert.Equal() Failure
                                  ↓ (pos 21)
Expected: ···\n  GivenNames: 'John',\n  FamilyName: 'Smith',\n  Spouse: 'Jill···
Actual:   ···\n  GivenNames: 'John James',\n  FamilyName: 'Smith',\n  Spouse:···
                                  ↑ (pos 21)
```
The clipboard will again contain the following:

> move /Y "C:\Code\Sample\SampleTest.Simple.received.txt" "C:\Code\Sample\SampleTest.Simple.verified.txt"

And the [Diff Tool](docs/diff-tool.md) is will display the diff:

![SecondDiff](/docs/SecondDiff.png)

The same approach can be used to verify the results and the change to `SampleTest.Simple.verified.txt` is committed to source control along with the change to `ClassBeingTested`.


### Disable Clipboard

The clipboard behavior can be disable using the following:

snippet: DisableClipboard


## Received and Verified

 * **All `*.verified.*` files should be committed to source control.**
 * **All `*.received.*` files should be excluded from source control.**


## Not valid json

Note that the output is technically not valid json. [Single quotes are used](docs/serializer-settings.md#single-quotes-used) and [names are not quoted](docs/serializer-settings.md#quotename-is-false). The reason for this is to make the resulting output easier to read and understand.


## Extensions

 * [Verify.Aspose](https://github.com/SimonCropp/Verify.Aspose): Extends Verify to allow verification of documents (pdf, docx, xslx, and pptx) via Aspose.
 * [Verify.NServiceBus](https://github.com/SimonCropp/Verify.NServiceBus): Adds Verify support to verify NServiceBus Test Contexts.
 * [Verify.Web](https://github.com/SimonCropp/Verify.Web): Extends Verify to allow verification of web bits.
 * [Verify.SqlServer](https://github.com/SimonCropp/Verify.SqlServer): Allow verification of SqlServer bits.
 * [Verify.EntityFramework](https://github.com/SimonCropp/Verify.EntityFramework): Extends Verify to allow verification of EntityFramework bits.


## Release Notes

See [closed milestones](../../milestones?state=closed).


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).