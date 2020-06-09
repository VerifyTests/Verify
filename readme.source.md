# <img src="/src/icon.png" height="30px"> Verify

[![Build status](https://ci.appveyor.com/api/projects/status/dpqylic0be7s9vnm/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/Verify)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Xunit.svg?label=Verify.Xunit)](https://www.nuget.org/packages/Verify.Xunit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.NUnit.svg?label=Verify.NUnit)](https://www.nuget.org/packages/Verify.NUnit/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.MSTest.svg?label=Verify.MSTest)](https://www.nuget.org/packages/Verify.MSTest/)

Verification tool to enable approval of complex models and documents.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

toc
include: doc-index


## NuGet packages

 * https://nuget.org/packages/Verify.Xunit/
 * https://nuget.org/packages/Verify.NUnit/
 * https://nuget.org/packages/Verify.MSTest/


## Verification versus Assertion

Given the following method:


#### Class being tested

snippet: ClassBeingTested


#### Tests

Compare a traditional assertion based test to a verification test.


##### Traditional assertion test:

snippet: TraditionalTest


##### Verification test

snippet: VerificationTest


#### Comparing Verification to Assertion

  * **Less test code**: verification test require less code to write.
  * **Reduced risk of incorrect test code**: Given the above assertion based test it would be difficult to ensure that no property is missing from the assertion. For example if a new property is added to the model. In the verification test that change would automatically be highlighted when the test is next run.
  * **Test failure visualization**: Verification test allows [visualization in a diff tool](https://github.com/VerifyTests/DiffEngine) that works for [complex models](/docs/SecondDiff.png) and [binary documents](/docs/binary.md).
  * **Multiple changes visualized in singe test run**: In the assertion approach, if multiple assertions require changing, this only becomes apparent over multiple test runs. In the verification approach, multiple changes can be [visualized in one test run](/docs/SecondDiff.png).
  * **Simpler creation of test "contract"**: In the assertion approach, complex models can require significant code to do the initial assertion. In the verification approach, the actual test and code-under-test can be used to create that "contract". See [initial verification](#initial-verification).
  * **Verification files committed to source control**: All resulting verified files are committed to source control in the most appropriate format. This means these files can be viewed at any time using any tooling. The files can also be diff'd over the history of the code base. This works for any file type, for example:
    * Html content can be committed as `.html` files.
    * Office documents can be committed as a rendered `.png` (see [Verify.Aspose](https://github.com/VerifyTests/Verify.Aspose)).
    * Database schema can be committed as `.sql` (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).


## Usage


### Class being tested

Given a class to be tested:

snippet: ClassBeingTested


### xUnit

Support for [xUnit](https://xunit.net/)

snippet: SampleTestXunit

 * [Verify Xunit Intro](https://www.youtube.com/watch?v=uGVogEltSkY)


### NUnit

Support for [NUnit](https://nunit.org/)

snippet: SampleTestNUnit


### MSTest

Support for [MSTest](https://github.com/Microsoft/testfx-docs)

snippet: SampleTestMSTest


### Initial Verification

When the test is initially run will fail with:

```
First verification. Sample.Test.verified.txt not found.
Verification command has been copied to the clipboard.
```

The clipboard will contain the following:

> cmd /c move /Y "C:\Code\Sample\Sample.Test.received.txt" "C:\Code\Sample\Sample.Test.verified.txt"

See also: [Clipboard](/docs/clipboard.md)

If a [Diff Tool](https://github.com/VerifyTests/DiffEngine) is detected it will display the diff:

![InitialDiff](/docs/InitialDiff.png)

To verify the result:

 * Execute the command from the clipboard, or
 * Use the diff tool to accept the changes, or
 * Manually copy the text to the new file


#### Verified result

This will result in the `Sample.Test.verified.txt` being created:

snippet: Verify.Xunit.Tests/Snippets/Sample.Test.verified.txt


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

> cmd /c move /Y "C:\Code\Sample\Sample.Test.received.txt" "C:\Code\Sample\Sample.Test.verified.txt"

See also: [Clipboard](/docs/clipboard.md)

And the [Diff Tool](https://github.com/VerifyTests/DiffEngine) is will display the diff:

![SecondDiff](/docs/SecondDiff.png)

The same approach can be used to verify the results and the change to `Sample.Test.verified.txt` is committed to source control along with the change to `ClassBeingTested`.


### AutoVerify

In some scenarios it makes sense to auto-accept any changes as part of a given test run. For example:

 * Keeping a text representation of a Database schema in a `.verified.sql` file (see [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer)).

This can be done using `AutoVerify()`:

snippet: AutoVerify

Note that auto accepted changes in `.verified.` files remain visible in source control tooling.


### OnHandlers

`OnFirstVerify` is called when there is no verified file.

`OnVerifyMismatch` is called when a received file does not match the existing verified file.

snippet: OnHandlers


## Received and Verified

 * **All `*.verified.*` files should be committed to source control.**
 * **All `*.received.*` files should be excluded from source control.**


## Not valid json

Note that the output is technically not valid json. [Single quotes are used](docs/serializer-settings.md#single-quotes-used) and [names are not quoted](docs/serializer-settings.md#quotename-is-false). The reason for this is to make the resulting output easier to read and understand.


## Videos

 * [Verify Xunit Intro](https://www.youtube.com/watch?v=uGVogEltSkY)


## Extensions

 * [Verify.AngleSharp.Diffing](https://github.com/VerifyTests/Verify.AngleSharp.Diffing): Comparison of html files via [AngleSharp.Diffing](https://github.com/AngleSharp/AngleSharp.Diffing).
 * [Verify.Aspose](https://github.com/VerifyTests/VerifyTests.Aspose): Verification of documents (pdf, docx, xslx, and pptx) via Aspose.
 * [Verify.EntityFramework](https://github.com/VerifyTests/Verify.EntityFramework): Verification of EntityFramework bits.
 * [Verify.ICSharpCode.Decompiler](https://github.com/VerifyTests/Verify.ICSharpCode.Decompiler): Comparison of assemblies and types via [ICSharpCode.Decompiler](https://github.com/icsharpcode/ILSpy/wiki/Getting-Started-With-ICSharpCode.Decompiler).
 * [Verify.ImageMagick](https://github.com/VerifyTests/Verify.ImageMagick): Verification and comparison of images via [Magick.NET](https://github.com/dlemstra/Magick.NET).
 * [Verify.ImageSharp](https://github.com/VerifyTests/Verify.ImageSharp): Verification of images via [ImageSharp](https://github.com/SixLabors/ImageSharp).
 * [Verify.NServiceBus](https://github.com/NServiceBusExtensions/Verify.NServiceBus): Verify NServiceBus Test Contexts.
 * [Verify.Phash](https://github.com/VerifyTests/Verify.Phash): Comparison of documents via [Phash](https://github.com/pgrho/phash).
 * [Verify.SqlServer](https://github.com/VerifyTests/Verify.SqlServer): Verification of SqlServer bits.
 * [Verify.Web](https://github.com/VerifyTests/Verify.Web): Verification of web bits.
 * [Verify.WinForms](https://github.com/VerifyTests/Verify.WinForms): Verification of WinForms UIs.
 * [Verify.Xaml](https://github.com/VerifyTests/Verify.Xaml): Verification of Xaml UIs.


## Alternatives

Projects/tools that may be a better alternative to Verify

 * [ApprovalTests](https://github.com/approvals/ApprovalTests.Net)
 * [Snapshooter](https://github.com/SwissLife-OSS/Snapshooter)
 * [Snapper](https://github.com/theramis/Snapper)


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Helmet](https://thenounproject.com/term/helmet/9554/) designed by [Leonidas Ikonomou](https://thenounproject.com/alterego) from [The Noun Project](https://thenounproject.com).